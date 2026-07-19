using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal sealed class VisualFloatRange
    {
        [JsonRequired]
        public float Minimum { get; set; }

        [JsonRequired]
        public float Maximum { get; set; }
    }

    internal sealed class VisualVector2Range
    {
        [JsonRequired]
        public VisualVector2 Minimum { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualVector2 Maximum { get; set; } = new VisualVector2();
    }

    internal enum VisualGradientMode
    {
        Blend,
        Fixed,
        PerceptualBlend,
    }

    internal enum VisualParticleGradientMode
    {
        Color,
        Gradient,
        TwoColors,
        TwoGradients,
        RandomColor,
    }

    internal sealed class VisualGradientColorKey
    {
        [JsonRequired]
        public float Time { get; set; }

        [JsonRequired]
        public string Color { get; set; } = string.Empty;
    }

    internal sealed class VisualGradientAlphaKey
    {
        [JsonRequired]
        public float Time { get; set; }

        [JsonRequired]
        public float Alpha { get; set; }
    }

    internal sealed class VisualGradient
    {
        [JsonRequired]
        public VisualGradientMode Mode { get; set; }

        [JsonRequired]
        public VisualGradientColorKey[] ColorKeys { get; set; } =
            System.Array.Empty<VisualGradientColorKey>();

        [JsonRequired]
        public VisualGradientAlphaKey[] AlphaKeys { get; set; } =
            System.Array.Empty<VisualGradientAlphaKey>();
    }

    internal sealed class VisualMinMaxGradient
    {
        [JsonRequired]
        public VisualParticleGradientMode Mode { get; set; }

        [McpOptional]
        public string? Color1 { get; set; }

        [McpOptional]
        public string? Color2 { get; set; }

        [McpOptional]
        public VisualGradient? Gradient1 { get; set; }

        [McpOptional]
        public VisualGradient? Gradient2 { get; set; }
    }
}
