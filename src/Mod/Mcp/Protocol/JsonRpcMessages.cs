using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ADOFAILoom.Mcp.Protocol
{
    internal sealed class JsonRpcSuccessResponse
    {
        public JsonRpcSuccessResponse(JsonElement id, object? result)
        {
            Id = id;
            Result = result;
        }

        [JsonPropertyName("jsonrpc")]
        public string JsonRpc => "2.0";

        [JsonPropertyName("id")]
        public JsonElement Id { get; }

        [JsonPropertyName("result")]
        public object? Result { get; }
    }

    internal sealed class JsonRpcErrorResponse
    {
        public JsonRpcErrorResponse(object? id, int code, string message, object? data = null)
        {
            Id = id;
            Error = new JsonRpcError(code, message, data);
        }

        [JsonPropertyName("jsonrpc")]
        public string JsonRpc => "2.0";

        [JsonPropertyName("id")]
        public object? Id { get; }

        [JsonPropertyName("error")]
        public JsonRpcError Error { get; }
    }

    internal sealed class JsonRpcError
    {
        public JsonRpcError(int code, string message, object? data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        [JsonPropertyName("code")]
        public int Code { get; }

        [JsonPropertyName("message")]
        public string Message { get; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; }
    }

    internal sealed class McpInitializeResult
    {
        public McpInitializeResult(string protocolVersion)
        {
            ProtocolVersion = protocolVersion;
        }

        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; }

        [JsonPropertyName("capabilities")]
        public McpServerCapabilities Capabilities { get; } = new McpServerCapabilities();

        [JsonPropertyName("serverInfo")]
        public McpServerInfo ServerInfo { get; } = new McpServerInfo();
    }

    internal sealed class McpServerCapabilities
    {
        [JsonPropertyName("tools")]
        public McpToolsCapability Tools { get; } = new McpToolsCapability();
    }

    internal sealed class McpToolsCapability
    {
        [JsonPropertyName("listChanged")]
        public bool ListChanged => false;
    }

    internal sealed class McpServerInfo
    {
        [JsonPropertyName("name")]
        public string Name => "ADOFAILoom";

        [JsonPropertyName("version")]
        public string Version => "1.1.0";
    }

    internal sealed class McpToolsListResult
    {
        public McpToolsListResult(IReadOnlyList<McpToolDefinition> tools)
        {
            Tools = tools;
        }

        [JsonPropertyName("tools")]
        public IReadOnlyList<McpToolDefinition> Tools { get; }
    }

    internal sealed class McpToolDefinition
    {
        public McpToolDefinition(
            string name,
            string description,
            JsonElement inputSchema,
            McpToolAnnotations annotations)
        {
            Name = name;
            Description = description;
            InputSchema = inputSchema;
            Annotations = annotations;
        }

        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("description")]
        public string Description { get; }

        [JsonPropertyName("inputSchema")]
        public JsonElement InputSchema { get; }

        [JsonPropertyName("annotations")]
        public McpToolAnnotations Annotations { get; }
    }

    internal sealed class McpToolAnnotations
    {
        public McpToolAnnotations(
            bool readOnly,
            bool destructive,
            bool idempotent,
            bool openWorld)
        {
            ReadOnlyHint = readOnly;
            DestructiveHint = destructive;
            IdempotentHint = idempotent;
            OpenWorldHint = openWorld;
        }

        [JsonPropertyName("readOnlyHint")]
        public bool ReadOnlyHint { get; }

        [JsonPropertyName("destructiveHint")]
        public bool DestructiveHint { get; }

        [JsonPropertyName("idempotentHint")]
        public bool IdempotentHint { get; }

        [JsonPropertyName("openWorldHint")]
        public bool OpenWorldHint { get; }
    }

    internal sealed class McpToolCallResult
    {
        public McpToolCallResult(
            IReadOnlyList<McpTextContent> content,
            bool isError,
            JsonElement? structuredContent = null)
        {
            Content = content;
            IsError = isError;
            StructuredContent = structuredContent;
        }

        [JsonPropertyName("content")]
        public IReadOnlyList<McpTextContent> Content { get; }

        [JsonPropertyName("isError")]
        public bool IsError { get; }

        [JsonPropertyName("structuredContent")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonElement? StructuredContent { get; }

        public static McpToolCallResult Success(JsonElement result)
        {
            return new McpToolCallResult(
                new[] { new McpTextContent(result.GetRawText()) },
                false,
                result);
        }

        public static McpToolCallResult Failure(string message)
        {
            return new McpToolCallResult(
                new[] { new McpTextContent(message) },
                true);
        }
    }

    internal sealed class McpTextContent
    {
        public McpTextContent(string text)
        {
            Text = text;
        }

        [JsonPropertyName("type")]
        public string Type => "text";

        [JsonPropertyName("text")]
        public string Text { get; }
    }

    internal sealed class McpHttpResponse
    {
        private McpHttpResponse(int statusCode, byte[]? body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public int StatusCode { get; }

        public byte[]? Body { get; }

        public static McpHttpResponse Json(
            int statusCode,
            object payload,
            JsonSerializerOptions options)
        {
            return new McpHttpResponse(
                statusCode,
                JsonSerializer.SerializeToUtf8Bytes(payload, payload.GetType(), options));
        }

        public static McpHttpResponse Empty(int statusCode)
        {
            return new McpHttpResponse(statusCode, null);
        }
    }
}
