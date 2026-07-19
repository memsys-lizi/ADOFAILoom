using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Protocol;

namespace ADOFAILoom.Mcp.Transport
{
    internal sealed class StreamableHttpTransport : IDisposable
    {
        private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(2);

        private readonly McpRequestRouter router;
        private readonly Action<string> log;
        private readonly object lifecycleGate = new object();
        private readonly ConcurrentDictionary<int, HttpListenerContext> activeContexts =
            new ConcurrentDictionary<int, HttpListenerContext>();
        private readonly ConcurrentDictionary<int, Task> activeRequests =
            new ConcurrentDictionary<int, Task>();

        private HttpListener? listener;
        private CancellationTokenSource? cancellation;
        private Task? acceptLoop;
        private int nextRequestId;

        public StreamableHttpTransport(McpRequestRouter router, Action<string> log)
        {
            this.router = router;
            this.log = log;
        }

        public void Start()
        {
            lock (lifecycleGate)
            {
                if (listener != null)
                {
                    throw new InvalidOperationException("The HTTP transport is already running.");
                }

                var newListener = new HttpListener();
                newListener.Prefixes.Add(McpProtocol.ListenerPrefix);
                try
                {
                    newListener.Start();
                }
                catch
                {
                    newListener.Close();
                    throw;
                }

                var newCancellation = new CancellationTokenSource();
                listener = newListener;
                cancellation = newCancellation;
                acceptLoop = Task.Run(
                    () => AcceptLoopAsync(newListener, newCancellation),
                    CancellationToken.None);
            }
        }

        public void Dispose()
        {
            HttpListener? listenerToStop;
            CancellationTokenSource? cancellationToStop;
            Task? acceptLoopToWait;

            lock (lifecycleGate)
            {
                listenerToStop = listener;
                cancellationToStop = cancellation;
                acceptLoopToWait = acceptLoop;
                listener = null;
                cancellation = null;
                acceptLoop = null;
            }

            if (listenerToStop == null || cancellationToStop == null)
            {
                return;
            }

            cancellationToStop.Cancel();
            listenerToStop.Close();
            CloseActiveResponses();

            WaitForShutdown(acceptLoopToWait, "HTTP accept loop");
            Task[] requests = activeRequests.Values.ToArray();
            if (requests.Length > 0)
            {
                WaitForShutdown(Task.WhenAll(requests), "active HTTP requests");
            }

            CloseActiveResponses();
            cancellationToStop.Dispose();
        }

        private async Task AcceptLoopAsync(
            HttpListener activeListener,
            CancellationTokenSource activeCancellation)
        {
            CancellationToken cancellationToken = activeCancellation.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await activeListener.GetContextAsync().ConfigureAwait(false);
                }
                catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception exception)
                {
                    log($"MCP HTTP accept loop stopped unexpectedly: {exception}");
                    activeCancellation.Cancel();
                    activeListener.Close();
                    return;
                }

                int requestId = Interlocked.Increment(ref nextRequestId);
                activeContexts.TryAdd(requestId, context);
                Task request = HandleContextAsync(requestId, context, cancellationToken);
                activeRequests.TryAdd(requestId, request);
                _ = request.ContinueWith(
                    completed =>
                    {
                        activeRequests.TryRemove(requestId, out _);
                        if (completed.IsFaulted && completed.Exception != null)
                        {
                            log($"MCP HTTP request task faulted: {completed.Exception.Flatten()}");
                        }
                    },
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }
        }

        private async Task HandleContextAsync(
            int requestId,
            HttpListenerContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                if (request.RemoteEndPoint == null ||
                    !IPAddress.IsLoopback(request.RemoteEndPoint.Address))
                {
                    await WriteEmptyAsync(context.Response, 403, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                }

                if (!string.Equals(
                        request.Url?.AbsolutePath,
                        McpProtocol.EndpointPath,
                        StringComparison.Ordinal))
                {
                    await WriteEmptyAsync(context.Response, 404, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                }

                if (!IsAllowedOrigin(request.Headers["Origin"]))
                {
                    await WriteEmptyAsync(context.Response, 403, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                }

                if (!string.Equals(request.HttpMethod, "POST", StringComparison.Ordinal))
                {
                    context.Response.Headers["Allow"] = "POST";
                    await WriteEmptyAsync(context.Response, 405, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                }

                if (!IsJsonContentType(request.ContentType))
                {
                    await WriteEmptyAsync(context.Response, 415, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                }

                string? accept = request.Headers["Accept"];
                if (!ContainsMediaType(accept, "application/json") ||
                    !ContainsMediaType(accept, "text/event-stream"))
                {
                    await WriteEmptyAsync(context.Response, 406, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                }

                if (request.ContentLength64 > McpProtocol.MaximumRequestBytes)
                {
                    await WriteEmptyAsync(context.Response, 413, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                }

                byte[] body;
                try
                {
                    body = await ReadBodyAsync(request.InputStream, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (RequestTooLargeException)
                {
                    await WriteEmptyAsync(context.Response, 413, cancellationToken)
                        .ConfigureAwait(false);
                    return;
                }

                McpHttpResponse response = await router
                    .HandleAsync(
                        body,
                        request.Headers[McpProtocol.ProtocolVersionHeader],
                        cancellationToken)
                    .ConfigureAwait(false);
                await WriteResponseAsync(context.Response, response, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (HttpListenerException)
            {
                // The client disconnected while the response was being written.
            }
            catch (IOException)
            {
                // The client disconnected while the request or response stream was active.
            }
            catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                log($"MCP HTTP request failed unexpectedly: {exception}");
                await TryWriteInternalErrorAsync(context.Response, cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                activeContexts.TryRemove(requestId, out _);
                TryClose(context.Response);
            }
        }

        private static async Task<byte[]> ReadBodyAsync(
            Stream input,
            CancellationToken cancellationToken)
        {
            using (var output = new MemoryStream())
            {
                var buffer = new byte[8192];
                while (true)
                {
                    int read = await input
                        .ReadAsync(buffer, 0, buffer.Length, cancellationToken)
                        .ConfigureAwait(false);
                    if (read == 0)
                    {
                        return output.ToArray();
                    }

                    if (output.Length + read > McpProtocol.MaximumRequestBytes)
                    {
                        throw new RequestTooLargeException();
                    }

                    output.Write(buffer, 0, read);
                }
            }
        }

        private static async Task WriteResponseAsync(
            HttpListenerResponse response,
            McpHttpResponse message,
            CancellationToken cancellationToken)
        {
            response.StatusCode = message.StatusCode;
            if (message.Body == null)
            {
                response.ContentLength64 = 0;
                response.Close();
                return;
            }

            response.ContentType = "application/json; charset=utf-8";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = message.Body.Length;
            await response.OutputStream
                .WriteAsync(message.Body, 0, message.Body.Length, cancellationToken)
                .ConfigureAwait(false);
            response.Close();
        }

        private static Task WriteEmptyAsync(
            HttpListenerResponse response,
            int statusCode,
            CancellationToken cancellationToken)
        {
            return WriteResponseAsync(
                response,
                McpHttpResponse.Empty(statusCode),
                cancellationToken);
        }

        private static async Task TryWriteInternalErrorAsync(
            HttpListenerResponse response,
            CancellationToken cancellationToken)
        {
            try
            {
                if (response.OutputStream == Stream.Null)
                {
                    return;
                }

                var payload = new JsonRpcErrorResponse(null, -32603, "Internal error");
                McpHttpResponse message = McpHttpResponse.Json(
                    500,
                    payload,
                    McpProtocol.JsonOptions);
                await WriteResponseAsync(response, message, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (HttpListenerException)
            {
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private static bool IsAllowedOrigin(string? origin)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                return true;
            }

            return Uri.TryCreate(origin, UriKind.Absolute, out Uri? uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
                   uri.IsLoopback;
        }

        private static bool IsJsonContentType(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            string[] parts = contentType.Split(';');
            if (!string.Equals(parts[0].Trim(), "application/json", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            foreach (string parameter in parts.Skip(1))
            {
                string[] pair = parameter.Split(new[] { '=' }, 2);
                if (pair.Length == 2 &&
                    string.Equals(pair[0].Trim(), "charset", StringComparison.OrdinalIgnoreCase))
                {
                    string charset = pair[1].Trim().Trim('"');
                    return string.Equals(charset, "utf-8", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(charset, "utf8", StringComparison.OrdinalIgnoreCase);
                }
            }

            return true;
        }

        private static bool ContainsMediaType(string? header, string mediaType)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                return false;
            }

            return header
                .Split(',')
                .Select(value => value.Split(';')[0].Trim())
                .Any(value => string.Equals(value, mediaType, StringComparison.OrdinalIgnoreCase));
        }

        private void CloseActiveResponses()
        {
            foreach (HttpListenerContext context in activeContexts.Values)
            {
                TryClose(context.Response);
            }
        }

        private void WaitForShutdown(Task? task, string operation)
        {
            if (task == null)
            {
                return;
            }

            try
            {
                if (!task.Wait(ShutdownTimeout))
                {
                    log($"Timed out while stopping {operation}.");
                }
            }
            catch (AggregateException exception)
            {
                Exception[] unexpected = exception
                    .Flatten()
                    .InnerExceptions
                    .Where(inner => inner is not OperationCanceledException &&
                                    inner is not ObjectDisposedException &&
                                    inner is not HttpListenerException &&
                                    inner is not IOException)
                    .ToArray();
                if (unexpected.Length > 0)
                {
                    log($"Error while stopping {operation}: {new AggregateException(unexpected)}");
                }
            }
        }

        private static void TryClose(HttpListenerResponse response)
        {
            try
            {
                response.Close();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (HttpListenerException)
            {
            }
        }

        private sealed class RequestTooLargeException : Exception
        {
        }
    }
}
