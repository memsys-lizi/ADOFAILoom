using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ADOFAI;
using ADOFAILoom.Mcp.Protocol;

namespace ADOFAILoom.State
{
    internal static class CanonicalJsonHash
    {
        public static string ComputeLevelRevision(LevelData levelData)
        {
            if (levelData == null)
            {
                throw new ArgumentNullException(nameof(levelData));
            }

            return ComputeJsonHash(levelData.Encode());
        }

        public static string ComputeEventFingerprint(LevelEvent levelEvent)
        {
            if (levelEvent == null)
            {
                throw new ArgumentNullException(nameof(levelEvent));
            }

            JsonElement encoded = JsonSerializer.SerializeToElement(
                levelEvent.Encode(),
                McpProtocol.JsonOptions
            );
            return ComputeElementHash(encoded);
        }

        public static JsonElement GetEventProperties(LevelEvent levelEvent)
        {
            var properties = new Dictionary<string, object>(levelEvent.Encode());
            properties.Remove("floor");
            properties.Remove("eventType");
            properties.Remove("active");
            properties.Remove("visible");
            properties.Remove("locked");
            return JsonSerializer.SerializeToElement(properties, McpProtocol.JsonOptions);
        }

        public static string ComputeJsonHash(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            using (JsonDocument document = JsonDocument.Parse(json))
            {
                return ComputeElementHash(document.RootElement);
            }
        }

        private static string ComputeElementHash(JsonElement element)
        {
            byte[] canonicalJson = WriteCanonicalJson(element);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(canonicalJson);
                var text = new StringBuilder(hash.Length * 2);
                foreach (byte value in hash)
                {
                    text.Append(value.ToString("x2"));
                }

                return text.ToString();
            }
        }

        private static byte[] WriteCanonicalJson(JsonElement element)
        {
            var text = new StringBuilder();
            WriteElement(text, element);
            return Encoding.UTF8.GetBytes(text.ToString());
        }

        private static void WriteElement(StringBuilder writer, JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    writer.Append('{');
                    bool firstProperty = true;
                    foreach (
                        JsonProperty property in element
                            .EnumerateObject()
                            .OrderBy(item => item.Name, StringComparer.Ordinal)
                    )
                    {
                        if (!firstProperty)
                        {
                            writer.Append(',');
                        }

                        firstProperty = false;
                        writer.Append(JsonSerializer.Serialize(property.Name));
                        writer.Append(':');
                        WriteElement(writer, property.Value);
                    }

                    writer.Append('}');
                    break;

                case JsonValueKind.Array:
                    writer.Append('[');
                    bool firstItem = true;
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        if (!firstItem)
                        {
                            writer.Append(',');
                        }

                        firstItem = false;
                        WriteElement(writer, item);
                    }

                    writer.Append(']');
                    break;

                case JsonValueKind.String:
                    writer.Append(JsonSerializer.Serialize(element.GetString()));
                    break;

                case JsonValueKind.Number:
                    writer.Append(element.GetRawText());
                    break;

                case JsonValueKind.True:
                    writer.Append("true");
                    break;

                case JsonValueKind.False:
                    writer.Append("false");
                    break;

                case JsonValueKind.Null:
                    writer.Append("null");
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported JSON value kind '{element.ValueKind}'."
                    );
            }
        }
    }
}
