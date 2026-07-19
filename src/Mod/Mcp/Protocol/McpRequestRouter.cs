using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Protocol
{
    internal sealed class McpRequestRouter
    {
        private const int ParseError = -32700;
        private const int InvalidRequest = -32600;
        private const int MethodNotFound = -32601;
        private const int InvalidParams = -32602;

        private readonly McpToolRegistry tools;
        private readonly Action<string> log;

        public McpRequestRouter(McpToolRegistry tools, Action<string> log)
        {
            this.tools = tools;
            this.log = log;
        }

        public async Task<McpHttpResponse> HandleAsync(
            byte[] body,
            string? protocolVersion,
            CancellationToken cancellationToken)
        {
            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(body, new JsonDocumentOptions
                {
                    AllowTrailingCommas = false,
                    CommentHandling = JsonCommentHandling.Disallow,
                    MaxDepth = 64
                });
            }
            catch (JsonException exception)
            {
                return Error(
                    400,
                    null,
                    ParseError,
                    "Parse error",
                    exception.Message);
            }

            using (document)
            {
                JsonElement root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object ||
                    !TryGetExactString(root, "jsonrpc", "2.0") ||
                    !TryGetRequiredString(root, "method", out string? method))
                {
                    return Error(400, null, InvalidRequest, "Invalid Request");
                }

                bool hasId = root.TryGetProperty("id", out JsonElement id);
                if (hasId && id.ValueKind != JsonValueKind.String &&
                    id.ValueKind != JsonValueKind.Number)
                {
                    return Error(400, null, InvalidRequest, "Invalid Request", "Invalid id.");
                }

                if (root.TryGetProperty("params", out JsonElement parameters) &&
                    parameters.ValueKind != JsonValueKind.Object)
                {
                    return hasId
                        ? Error(200, id, InvalidParams, "Invalid params", "params must be an object.")
                        : McpHttpResponse.Empty(400);
                }

                if (!string.Equals(method, "initialize", StringComparison.Ordinal) &&
                    !McpProtocol.IsSupportedVersion(protocolVersion))
                {
                    string detail = protocolVersion == null
                        ? $"Missing required {McpProtocol.ProtocolVersionHeader} header."
                        : $"Unsupported MCP protocol version '{protocolVersion}'.";
                    return hasId
                        ? Error(400, id, InvalidRequest, "Invalid Request", detail)
                        : McpHttpResponse.Empty(400);
                }

                if (!hasId)
                {
                    return HandleNotification(method!, root);
                }

                switch (method)
                {
                    case "initialize":
                        return HandleInitialize(id, root);
                    case "ping":
                        return HandlePing(id, root);
                    case "tools/list":
                        return HandleToolsList(id, root);
                    case "tools/call":
                        return await HandleToolsCallAsync(
                                id,
                                root,
                                cancellationToken)
                            .ConfigureAwait(false);
                    default:
                        return Error(200, id, MethodNotFound, "Method not found", method);
                }
            }
        }

        private static McpHttpResponse HandleNotification(string method, JsonElement root)
        {
            if (!string.Equals(method, "notifications/initialized", StringComparison.Ordinal))
            {
                return McpHttpResponse.Empty(202);
            }

            if (!TryValidateOptionalParameters(root, out _))
            {
                return McpHttpResponse.Empty(400);
            }

            return McpHttpResponse.Empty(202);
        }

        private static McpHttpResponse HandleInitialize(JsonElement id, JsonElement root)
        {
            if (!root.TryGetProperty("params", out JsonElement parameters))
            {
                return Error(
                    200,
                    id,
                    InvalidParams,
                    "Invalid params",
                    "initialize requires parameters.");
            }

            if (!McpRequestParameters.TryValidate(
                    parameters,
                    out string? parameterError,
                    "protocolVersion",
                    "capabilities",
                    "clientInfo"))
            {
                return Error(200, id, InvalidParams, "Invalid params", parameterError);
            }

            if (!TryGetRequiredString(
                    parameters,
                    "protocolVersion",
                    out string? requestedVersion) ||
                !parameters.TryGetProperty("capabilities", out JsonElement capabilities) ||
                capabilities.ValueKind != JsonValueKind.Object ||
                !parameters.TryGetProperty("clientInfo", out JsonElement clientInfo) ||
                clientInfo.ValueKind != JsonValueKind.Object)
            {
                return Error(
                    200,
                    id,
                    InvalidParams,
                    "Invalid params",
                    "initialize requires protocolVersion, capabilities, and clientInfo.");
            }

            string negotiatedVersion = McpProtocol.NegotiateVersion(requestedVersion!);
            return Success(id, new McpInitializeResult(negotiatedVersion));
        }

        private static McpHttpResponse HandlePing(JsonElement id, JsonElement root)
        {
            return TryValidateOptionalParameters(root, out string? parameterError)
                ? Success(id, new Dictionary<string, object?>())
                : Error(200, id, InvalidParams, "Invalid params", parameterError);
        }

        private McpHttpResponse HandleToolsList(JsonElement id, JsonElement root)
        {
            if (!TryValidateOptionalParameters(root, out string? parameterError))
            {
                return Error(
                    200,
                    id,
                    InvalidParams,
                    "Invalid params",
                    parameterError);
            }

            return Success(id, new McpToolsListResult(tools.Definitions));
        }

        private async Task<McpHttpResponse> HandleToolsCallAsync(
            JsonElement id,
            JsonElement root,
            CancellationToken cancellationToken)
        {
            if (!root.TryGetProperty("params", out JsonElement parameters) ||
                !TryGetRequiredString(parameters, "name", out string? name))
            {
                return Error(
                    200,
                    id,
                    InvalidParams,
                    "Invalid params",
                    "tools/call requires a tool name.");
            }

            if (!McpRequestParameters.TryValidate(
                    parameters,
                    out string? parameterError,
                    "name",
                    "arguments",
                    "task"))
            {
                return Error(
                    200,
                    id,
                    InvalidParams,
                    "Invalid params",
                    parameterError);
            }

            if (parameters.TryGetProperty("task", out _))
            {
                return Error(
                    200,
                    id,
                    InvalidParams,
                    "Invalid params",
                    "This server does not support task-augmented tool execution.");
            }

            JsonElement? arguments = null;
            if (parameters.TryGetProperty("arguments", out JsonElement argumentElement) &&
                argumentElement.ValueKind != JsonValueKind.Null)
            {
                if (argumentElement.ValueKind != JsonValueKind.Object)
                {
                    return Error(
                        200,
                        id,
                        InvalidParams,
                        "Invalid params",
                        "Tool arguments must be an object.");
                }

                arguments = argumentElement;
            }

            if (!tools.TryGet(name!, out McpToolInvoker? invoker) || invoker == null)
            {
                return Error(
                    200,
                    id,
                    InvalidParams,
                    "Invalid params",
                    $"Unknown tool '{name}'.");
            }

            try
            {
                JsonElement result = await invoker
                    .InvokeAsync(arguments, cancellationToken)
                    .ConfigureAwait(false);
                return Success(id, McpToolCallResult.Success(result));
            }
            catch (McpInvalidArgumentsException exception)
            {
                return Error(200, id, InvalidParams, "Invalid params", exception.Message);
            }
            catch (ArgumentException exception)
            {
                return Error(200, id, InvalidParams, "Invalid params", exception.Message);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                log($"MCP tool '{name}' failed: {exception}");
                return Success(id, McpToolCallResult.Failure(exception.Message));
            }
        }

        private static bool TryValidateOptionalParameters(
            JsonElement root,
            out string? error)
        {
            if (!root.TryGetProperty("params", out JsonElement parameters))
            {
                error = null;
                return true;
            }

            return McpRequestParameters.TryValidate(parameters, out error);
        }

        private static bool TryGetExactString(
            JsonElement element,
            string propertyName,
            string expected)
        {
            return element.TryGetProperty(propertyName, out JsonElement property) &&
                   property.ValueKind == JsonValueKind.String &&
                   string.Equals(property.GetString(), expected, StringComparison.Ordinal);
        }

        private static bool TryGetRequiredString(
            JsonElement element,
            string propertyName,
            out string? value)
        {
            value = null;
            if (!element.TryGetProperty(propertyName, out JsonElement property) ||
                property.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            value = property.GetString();
            return !string.IsNullOrWhiteSpace(value);
        }

        private static McpHttpResponse Success(JsonElement id, object? result)
        {
            return McpHttpResponse.Json(
                200,
                new JsonRpcSuccessResponse(id, result),
                McpProtocol.JsonOptions);
        }

        private static McpHttpResponse Error(
            int statusCode,
            object? id,
            int code,
            string message,
            object? data = null)
        {
            return McpHttpResponse.Json(
                statusCode,
                new JsonRpcErrorResponse(id, code, message, data),
                McpProtocol.JsonOptions);
        }
    }
}
