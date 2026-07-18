namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class McpToolCallResult
    {
        private McpToolCallResult(object? response, string? invalidParametersMessage)
        {
            Response = response;
            InvalidParametersMessage = invalidParametersMessage;
        }

        public object? Response { get; }

        public string? InvalidParametersMessage { get; }

        public bool HasInvalidParameters => InvalidParametersMessage != null;

        public static McpToolCallResult Success<T>(T structuredContent, string text)
        {
            return new McpToolCallResult(new
            {
                content = new[]
                {
                    new { type = "text", text }
                },
                structuredContent,
                isError = false
            }, null);
        }

        public static McpToolCallResult ExecutionError(string message)
        {
            return new McpToolCallResult(new
            {
                content = new[]
                {
                    new { type = "text", text = message }
                },
                isError = true
            }, null);
        }

        public static McpToolCallResult InvalidParameters(string message)
        {
            return new McpToolCallResult(null, message);
        }
    }
}
