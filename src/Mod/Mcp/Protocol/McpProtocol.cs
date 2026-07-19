using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ADOFAILoom.Mcp.Protocol
{
    internal static class McpProtocol
    {
        public const string ListenerPrefix = "http://127.0.0.1:39473/";
        public const string EndpointPath = "/mcp";
        public const string ProtocolVersionHeader = "MCP-Protocol-Version";
        public const int MaximumRequestBytes = 64 * 1024;

        public static readonly TimeSpan MainThreadTimeout = TimeSpan.FromSeconds(5);

        private static readonly HashSet<string> SupportedVersions = new HashSet<string>(
            StringComparer.Ordinal
        )
        {
            "2025-03-26",
            "2025-06-18",
            "2025-11-25",
        };

        public static JsonSerializerOptions JsonOptions { get; } = CreateJsonOptions();

        public static bool IsSupportedVersion(string? version)
        {
            return version != null && SupportedVersions.Contains(version);
        }

        public static string NegotiateVersion(string requestedVersion)
        {
            return IsSupportedVersion(requestedVersion) ? requestedVersion : "2025-11-25";
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = false,
                AllowTrailingCommas = false,
                ReadCommentHandling = JsonCommentHandling.Disallow,
                UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                MaxDepth = 64,
            };
            options.Converters.Add(new JsonStringEnumConverter(null, false));
            return options;
        }
    }
}
