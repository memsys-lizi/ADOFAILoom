using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Tools;

namespace ADOFAILoom.Mcp
{
    internal sealed class McpRequestHandler
    {
        private readonly McpToolRegistry toolRegistry;

        public McpRequestHandler(McpToolRegistry toolRegistry)
        {
            this.toolRegistry = toolRegistry;
        }

        public async Task<McpHttpResult> HandleAsync(
            string requestJson,
            CancellationToken cancellationToken)
        {
            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(requestJson);
            }
            catch (JsonException)
            {
                return Error(null, -32700, "Parse error", 400);
            }

            using (document)
            {
                JsonElement root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object
                    || !root.TryGetProperty("jsonrpc", out JsonElement jsonRpc)
                    || jsonRpc.ValueKind != JsonValueKind.String
                    || jsonRpc.GetString() != "2.0")
                {
                    return Error(GetIdOrNull(root), -32600, "Invalid Request", 400);
                }

                bool hasId = root.TryGetProperty("id", out JsonElement idElement);
                if (hasId
                    && idElement.ValueKind != JsonValueKind.String
                    && idElement.ValueKind != JsonValueKind.Number
                    && idElement.ValueKind != JsonValueKind.Null)
                {
                    return Error(null, -32600, "Invalid Request", 400);
                }

                object? id = hasId ? idElement.Clone() : null;

                if (!root.TryGetProperty("method", out JsonElement methodElement)
                    || methodElement.ValueKind != JsonValueKind.String)
                {
                    // Clients may POST JSON-RPC responses. This server never sends requests,
                    // so accepting and ignoring them is sufficient.
                    return hasId
                        ? Error(id, -32600, "Invalid Request", 400)
                        : McpHttpResult.Accepted();
                }

                string method = methodElement.GetString() ?? string.Empty;
                if (!hasId)
                {
                    return McpHttpResult.Accepted();
                }

                switch (method)
                {
                    case "initialize":
                        return HandleInitialize(root, id);
                    case "ping":
                        return Success(id, new { });
                    case "tools/list":
                        return HandleToolsList(id);
                    case "tools/call":
                        return await HandleToolCallAsync(root, id, cancellationToken)
                            .ConfigureAwait(false);
                    default:
                        return Error(id, -32601, $"Method not found: {method}");
                }
            }
        }

        private static McpHttpResult HandleInitialize(JsonElement root, object? id)
        {
            string? requestedVersion = null;
            if (root.TryGetProperty("params", out JsonElement parameters)
                && parameters.ValueKind == JsonValueKind.Object
                && parameters.TryGetProperty("protocolVersion", out JsonElement versionElement)
                && versionElement.ValueKind == JsonValueKind.String)
            {
                requestedVersion = versionElement.GetString();
            }

            string negotiatedVersion = McpConstants.IsSupportedProtocolVersion(requestedVersion)
                ? requestedVersion!
                : McpConstants.LatestProtocolVersion;

            return Success(id, new
            {
                protocolVersion = negotiatedVersion,
                capabilities = new
                {
                    tools = new { listChanged = false }
                },
                serverInfo = new
                {
                    name = McpConstants.ServerName,
                    version = McpConstants.ServerVersion
                },
                instructions = "Read-only access to the current A Dance of Fire and Ice game state."
            });
        }

        private McpHttpResult HandleToolsList(object? id)
        {
            return Success(id, new { tools = toolRegistry.GetDefinitions() });
        }

        private async Task<McpHttpResult> HandleToolCallAsync(
            JsonElement root,
            object? id,
            CancellationToken cancellationToken)
        {
            if (!root.TryGetProperty("params", out JsonElement parameters)
                || parameters.ValueKind != JsonValueKind.Object
                || !parameters.TryGetProperty("name", out JsonElement nameElement)
                || nameElement.ValueKind != JsonValueKind.String)
            {
                return Error(id, -32602, "Missing or invalid tool name.");
            }

            string toolName = nameElement.GetString() ?? string.Empty;
            if (!toolRegistry.TryGet(toolName, out IMcpTool tool))
            {
                return Error(id, -32602, $"Unknown tool: {toolName}");
            }

            JsonElement? arguments = parameters.TryGetProperty(
                "arguments",
                out JsonElement argumentsElement)
                    ? argumentsElement
                    : (JsonElement?)null;

            McpToolCallResult callResult;
            try
            {
                callResult = await tool.ExecuteAsync(arguments, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                callResult = McpToolCallResult.ExecutionError(
                    $"Tool {toolName} failed: {exception.Message}");
            }

            if (callResult.HasInvalidParameters)
            {
                return Error(id, -32602, callResult.InvalidParametersMessage!);
            }

            return Success(id, callResult.Response!);
        }

        private static McpHttpResult Success(object? id, object result)
        {
            return McpHttpResult.JsonResponse(McpJson.Serialize(new
            {
                jsonrpc = "2.0",
                id,
                result
            }));
        }

        private static McpHttpResult Error(
            object? id,
            int code,
            string message,
            int statusCode = 200)
        {
            return McpHttpResult.JsonResponse(McpJson.Serialize(new
            {
                jsonrpc = "2.0",
                id,
                error = new { code, message }
            }), statusCode);
        }

        private static object? GetIdOrNull(JsonElement root)
        {
            return root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty("id", out JsonElement id)
                    ? id.Clone()
                    : null;
        }
    }
}
