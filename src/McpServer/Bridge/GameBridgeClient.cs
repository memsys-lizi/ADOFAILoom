using System.IO.Pipes;
using System.Text.Json;
using ADOFAILoom.Protocol;
using ModelContextProtocol;

namespace ADOFAILoom.McpServer.Bridge;

public sealed class GameBridgeClient
{
    public async Task<GameState> GetGameStateAsync(CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(BridgeProtocol.RequestTimeoutMilliseconds);
        bool bridgeConnected = false;

        try
        {
            using var pipe = new NamedPipeClientStream(
                ".",
                BridgeProtocol.PipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);

            await pipe.ConnectAsync(timeout.Token).ConfigureAwait(false);
            bridgeConnected = true;

            string requestId = Guid.NewGuid().ToString("N");
            await PipeMessageIO.WriteAsync(
                pipe,
                new BridgeRequest
                {
                    RequestId = requestId,
                    Method = BridgeProtocol.GetGameStateMethod
                },
                timeout.Token).ConfigureAwait(false);

            BridgeResponse response = await PipeMessageIO
                .ReadAsync<BridgeResponse>(pipe, timeout.Token)
                .ConfigureAwait(false);
            ValidateResponse(response, requestId);
            return response.Result!;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException) when (!bridgeConnected)
        {
            return GameState.Disconnected();
        }
        catch (TimeoutException) when (!bridgeConnected)
        {
            return GameState.Disconnected();
        }
        catch (IOException) when (!bridgeConnected)
        {
            return GameState.Disconnected();
        }
        catch (OperationCanceledException)
        {
            throw new McpException("The game bridge did not answer within 2 seconds.");
        }
        catch (TimeoutException exception)
        {
            throw new McpException($"The game bridge timed out: {exception.Message}");
        }
        catch (IOException exception)
        {
            throw new McpException($"The game bridge connection failed: {exception.Message}");
        }
        catch (JsonException exception)
        {
            throw new McpException($"The game bridge returned invalid JSON: {exception.Message}");
        }
        catch (InvalidDataException exception)
        {
            throw new McpException($"The game bridge returned an invalid response: {exception.Message}");
        }
    }

    private static void ValidateResponse(BridgeResponse response, string requestId)
    {
        if (response.ProtocolVersion != BridgeProtocol.Version)
        {
            throw new McpException(
                $"Unsupported game bridge protocol version: {response.ProtocolVersion}.");
        }

        if (!string.Equals(response.RequestId, requestId, StringComparison.Ordinal))
        {
            throw new McpException("The game bridge response requestId does not match.");
        }

        if (response.Error != null)
        {
            throw new McpException(
                $"Game bridge error {response.Error.Code}: {response.Error.Message}");
        }

        if (response.Result == null)
        {
            throw new McpException("The game bridge response has no result.");
        }
    }
}
