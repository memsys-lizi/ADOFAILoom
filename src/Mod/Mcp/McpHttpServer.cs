using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADOFAILoom.Mcp
{
    internal sealed class McpHttpServer : IDisposable
    {
        private readonly McpRequestHandler requestHandler;
        private readonly Action<string> log;
        private readonly ConcurrentDictionary<HttpListenerContext, byte> activeContexts =
            new ConcurrentDictionary<HttpListenerContext, byte>();
        private HttpListener? listener;
        private CancellationTokenSource? cancellation;
        private Task? listenLoop;

        public McpHttpServer(McpRequestHandler requestHandler, Action<string> log)
        {
            this.requestHandler = requestHandler;
            this.log = log;
        }

        public void Start()
        {
            if (listener != null)
            {
                return;
            }

            var newListener = new HttpListener();
            newListener.Prefixes.Add(McpConstants.ListenerPrefix);
            newListener.Start();

            listener = newListener;
            cancellation = new CancellationTokenSource();
            listenLoop = Task.Run(() => ListenAsync(newListener, cancellation.Token));
        }

        private async Task ListenAsync(HttpListener currentListener, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    HttpListenerContext context = await currentListener.GetContextAsync()
                        .ConfigureAwait(false);
                    activeContexts.TryAdd(context, 0);
                    _ = HandleAndCloseAsync(context, cancellationToken);
                }
                catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    log($"MCP HTTP accept error: {exception.Message}");
                }
            }
        }

        private async Task HandleAndCloseAsync(
            HttpListenerContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                await HandleAsync(context, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Normal shutdown.
            }
            catch (Exception exception)
            {
                log($"MCP HTTP request error: {exception.Message}");
                TrySetStatusCode(context.Response, 500);
            }
            finally
            {
                activeContexts.TryRemove(context, out _);
                try
                {
                    context.Response.Close();
                }
                catch
                {
                    // The client may already have closed the connection.
                }
            }
        }

        private async Task HandleAsync(
            HttpListenerContext context,
            CancellationToken cancellationToken)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            response.Headers["Cache-Control"] = "no-store";
            response.Headers["X-Content-Type-Options"] = "nosniff";

            if (request.RemoteEndPoint == null || !IPAddress.IsLoopback(request.RemoteEndPoint.Address))
            {
                response.StatusCode = 403;
                return;
            }

            if (!IsAllowedOrigin(request.Headers["Origin"]))
            {
                response.StatusCode = 403;
                return;
            }

            string path = request.Url?.AbsolutePath.TrimEnd('/') ?? string.Empty;
            if (!string.Equals(path, McpConstants.EndpointPath, StringComparison.Ordinal))
            {
                response.StatusCode = 404;
                return;
            }

            if (request.HttpMethod == "GET" || request.HttpMethod == "DELETE")
            {
                response.StatusCode = 405;
                response.Headers["Allow"] = "POST";
                return;
            }

            if (request.HttpMethod != "POST")
            {
                response.StatusCode = 405;
                response.Headers["Allow"] = "POST";
                return;
            }

            if (request.ContentType == null
                || !request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = 415;
                return;
            }

            string? protocolVersion = request.Headers["MCP-Protocol-Version"];
            if (!string.IsNullOrWhiteSpace(protocolVersion)
                && !McpConstants.IsSupportedProtocolVersion(protocolVersion))
            {
                await WriteJsonAsync(
                    response,
                    "{\"jsonrpc\":\"2.0\",\"id\":null,\"error\":{\"code\":-32600,\"message\":\"Unsupported MCP-Protocol-Version header.\"}}",
                    400,
                    cancellationToken).ConfigureAwait(false);
                return;
            }

            string requestJson;
            try
            {
                requestJson = await ReadBodyAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidDataException)
            {
                response.StatusCode = 413;
                return;
            }

            McpHttpResult result = await requestHandler.HandleAsync(requestJson, cancellationToken)
                .ConfigureAwait(false);
            if (result.Json == null)
            {
                response.StatusCode = result.StatusCode;
                return;
            }

            await WriteJsonAsync(response, result.Json, result.StatusCode, cancellationToken)
                .ConfigureAwait(false);
        }

        private static bool IsAllowedOrigin(string? origin)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                return true;
            }

            return Uri.TryCreate(origin, UriKind.Absolute, out Uri uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                && uri.IsLoopback;
        }

        private static async Task<string> ReadBodyAsync(
            HttpListenerRequest request,
            CancellationToken cancellationToken)
        {
            if (request.ContentLength64 > McpConstants.MaxRequestBytes)
            {
                throw new InvalidDataException("MCP request is too large.");
            }

            using (var buffer = new MemoryStream())
            {
                var chunk = new byte[4096];
                while (true)
                {
                    int count = await request.InputStream
                        .ReadAsync(chunk, 0, chunk.Length, cancellationToken)
                        .ConfigureAwait(false);
                    if (count == 0)
                    {
                        break;
                    }

                    if (buffer.Length + count > McpConstants.MaxRequestBytes)
                    {
                        throw new InvalidDataException("MCP request is too large.");
                    }

                    buffer.Write(chunk, 0, count);
                }

                return Encoding.UTF8.GetString(buffer.ToArray());
            }
        }

        private static async Task WriteJsonAsync(
            HttpListenerResponse response,
            string json,
            int statusCode,
            CancellationToken cancellationToken)
        {
            byte[] payload = Encoding.UTF8.GetBytes(json);
            response.StatusCode = statusCode;
            response.ContentType = "application/json; charset=utf-8";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = payload.Length;
            await response.OutputStream.WriteAsync(payload, 0, payload.Length, cancellationToken)
                .ConfigureAwait(false);
        }

        private static void TrySetStatusCode(HttpListenerResponse response, int statusCode)
        {
            try
            {
                response.StatusCode = statusCode;
            }
            catch
            {
                // The response may already be committed or closed.
            }
        }

        public void Dispose()
        {
            HttpListener? currentListener = listener;
            CancellationTokenSource? currentCancellation = cancellation;
            Task? currentListenLoop = listenLoop;
            listener = null;
            cancellation = null;
            listenLoop = null;

            if (currentListener == null || currentCancellation == null)
            {
                return;
            }

            currentCancellation.Cancel();
            currentListener.Close();
            foreach (HttpListenerContext context in activeContexts.Keys)
            {
                try
                {
                    context.Response.Abort();
                }
                catch
                {
                    // Best-effort shutdown for disconnected clients.
                }
            }

            try
            {
                currentListenLoop?.Wait(500);
            }
            catch (AggregateException)
            {
                // Listener shutdown commonly completes through cancellation/disposal.
            }

            currentCancellation.Dispose();
        }
    }
}
