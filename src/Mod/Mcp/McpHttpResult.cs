namespace ADOFAILoom.Mcp
{
    internal sealed class McpHttpResult
    {
        private McpHttpResult(int statusCode, string? json)
        {
            StatusCode = statusCode;
            Json = json;
        }

        public int StatusCode { get; }

        public string? Json { get; }

        public static McpHttpResult JsonResponse(string json, int statusCode = 200)
        {
            return new McpHttpResult(statusCode, json);
        }

        public static McpHttpResult Accepted()
        {
            return new McpHttpResult(202, null);
        }
    }
}
