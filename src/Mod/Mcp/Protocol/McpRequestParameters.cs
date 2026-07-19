using System;
using System.Text.Json;

namespace ADOFAILoom.Mcp.Protocol
{
    internal static class McpRequestParameters
    {
        private const string MetadataName = "_meta";
        private const string ProgressTokenName = "progressToken";

        public static bool TryValidate(
            JsonElement parameters,
            out string? error,
            params string[] applicationParameterNames)
        {
            foreach (JsonProperty property in parameters.EnumerateObject())
            {
                if (string.Equals(property.Name, MetadataName, StringComparison.Ordinal))
                {
                    if (!TryValidateMetadata(property.Value, out error))
                    {
                        return false;
                    }

                    continue;
                }

                if (!Contains(applicationParameterNames, property.Name))
                {
                    error = $"Unknown parameter '{property.Name}'.";
                    return false;
                }
            }

            error = null;
            return true;
        }

        private static bool TryValidateMetadata(JsonElement metadata, out string? error)
        {
            if (metadata.ValueKind != JsonValueKind.Object)
            {
                error = "_meta must be an object.";
                return false;
            }

            if (metadata.TryGetProperty(ProgressTokenName, out JsonElement progressToken) &&
                progressToken.ValueKind != JsonValueKind.String &&
                progressToken.ValueKind != JsonValueKind.Number)
            {
                error = "_meta.progressToken must be a string or number.";
                return false;
            }

            error = null;
            return true;
        }

        private static bool Contains(string[] names, string candidate)
        {
            foreach (string name in names)
            {
                if (string.Equals(name, candidate, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
