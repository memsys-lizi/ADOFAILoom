using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Protocol;
using ADOFAILoom.State;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Bridge
{
    internal sealed class GameBridgeServer : IDisposable
    {
        private readonly MainThreadDispatcher dispatcher;
        private readonly Action<string> log;
        private readonly ConcurrentDictionary<NamedPipeServerStream, byte> activePipes =
            new ConcurrentDictionary<NamedPipeServerStream, byte>();
        private CancellationTokenSource? cancellation;
        private Task? acceptLoop;

        public GameBridgeServer(MainThreadDispatcher dispatcher, Action<string> log)
        {
            this.dispatcher = dispatcher;
            this.log = log;
        }

        public void Start()
        {
            if (cancellation != null)
            {
                return;
            }

            cancellation = new CancellationTokenSource();
            acceptLoop = Task.Run(() => AcceptLoopAsync(cancellation.Token));
        }

        private async Task AcceptLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var pipe = new NamedPipeServerStream(
                    BridgeProtocol.PipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);
                activePipes.TryAdd(pipe, 0);

                try
                {
                    await pipe.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
                    _ = HandleAndCloseAsync(pipe, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    ClosePipe(pipe);
                    break;
                }
                catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
                {
                    ClosePipe(pipe);
                    break;
                }
                catch (Exception exception)
                {
                    ClosePipe(pipe);
                    log($"Game bridge accept error: {exception.Message}");
                }
            }
        }

        private async Task HandleAndCloseAsync(
            NamedPipeServerStream pipe,
            CancellationToken cancellationToken)
        {
            try
            {
                await HandleAsync(pipe, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            catch (OperationCanceledException)
            {
                // A client request reached its time limit.
            }
            catch (IOException)
            {
                // Clients may disconnect before reading the response.
            }
            catch (Exception exception)
            {
                log($"Game bridge request error: {exception.Message}");
            }
            finally
            {
                ClosePipe(pipe);
            }
        }

        private async Task HandleAsync(
            NamedPipeServerStream pipe,
            CancellationToken cancellationToken)
        {
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                timeout.CancelAfter(BridgeProtocol.RequestTimeoutMilliseconds);

                BridgeRequest request;
                try
                {
                    request = await PipeMessageIO.ReadAsync<BridgeRequest>(pipe, timeout.Token)
                        .ConfigureAwait(false);
                }
                catch (Exception exception) when (
                    exception is InvalidDataException || exception is JsonException)
                {
                    await PipeMessageIO.WriteAsync(
                        pipe,
                        BridgeResponse.Failure(null, "invalid_request", exception.Message),
                        timeout.Token).ConfigureAwait(false);
                    return;
                }

                BridgeResponse response = await ExecuteAsync(request, timeout.Token)
                    .ConfigureAwait(false);
                await PipeMessageIO.WriteAsync(pipe, response, timeout.Token)
                    .ConfigureAwait(false);
            }
        }

        private async Task<BridgeResponse> ExecuteAsync(
            BridgeRequest request,
            CancellationToken cancellationToken)
        {
            if (request.ProtocolVersion != BridgeProtocol.Version)
            {
                return BridgeResponse.Failure(
                    request.RequestId,
                    "unsupported_protocol_version",
                    $"Bridge protocol version {request.ProtocolVersion} is not supported.");
            }

            if (string.IsNullOrWhiteSpace(request.RequestId))
            {
                return BridgeResponse.Failure(null, "invalid_request", "requestId is required.");
            }

            if (request.Method != BridgeProtocol.GetGameStateMethod)
            {
                return BridgeResponse.Failure(
                    request.RequestId,
                    "method_not_found",
                    $"Unknown bridge method: {request.Method}");
            }

            GameState state = await dispatcher
                .InvokeAsync(GameStateSnapshotProvider.Capture, cancellationToken)
                .ConfigureAwait(false);
            return BridgeResponse.Success(request.RequestId, state);
        }

        private void ClosePipe(NamedPipeServerStream pipe)
        {
            activePipes.TryRemove(pipe, out _);
            pipe.Dispose();
        }

        public void Dispose()
        {
            CancellationTokenSource? currentCancellation = cancellation;
            Task? currentAcceptLoop = acceptLoop;
            cancellation = null;
            acceptLoop = null;

            if (currentCancellation == null)
            {
                return;
            }

            currentCancellation.Cancel();
            foreach (NamedPipeServerStream pipe in activePipes.Keys)
            {
                ClosePipe(pipe);
            }

            try
            {
                currentAcceptLoop?.Wait(500);
            }
            catch (AggregateException)
            {
                // Cancellation/disposal is the expected shutdown path.
            }

            currentCancellation.Dispose();
        }
    }
}
