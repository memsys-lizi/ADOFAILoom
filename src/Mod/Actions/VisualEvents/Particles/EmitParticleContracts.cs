using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class EmitParticleEventCreate : OffsetEventCreate
    {
        [JsonRequired]
        public string Tag { get; set; } = string.Empty;

        [JsonRequired]
        public int Count { get; set; }
    }

    internal sealed class EmitParticleEventUpdate : OffsetEventUpdate
    {
        [McpOptional]
        public string? Tag { get; set; }

        [McpOptional]
        public int? Count { get; set; }
    }
}
