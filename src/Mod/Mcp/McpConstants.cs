namespace ADOFAILoom.Mcp
{
    internal static class McpConstants
    {
        public const string ServerName = "adofai-loom";
        public const string ServerVersion = "1.0.0";
        public const string LatestProtocolVersion = "2025-11-25";
        public const string EndpointUrl = "http://127.0.0.1:39473/mcp";
        public const string ListenerPrefix = "http://127.0.0.1:39473/";
        public const string EndpointPath = "/mcp";
        public const int MaxRequestBytes = 64 * 1024;
        public const int ToolTimeoutMilliseconds = 5000;

        public static bool IsSupportedProtocolVersion(string? version)
        {
            return version == "2025-03-26"
                || version == "2025-06-18"
                || version == LatestProtocolVersion;
        }
    }
}
