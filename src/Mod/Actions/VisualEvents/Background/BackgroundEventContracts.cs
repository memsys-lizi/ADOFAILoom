using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualBackgroundDisplayMode
    {
        FitToScreen,
        Unscaled,
        Tiled,
    }

    internal sealed class CustomBackgroundEventCreate : OffsetEventCreate
    {
        [JsonRequired]
        public string Color { get; set; } = string.Empty;

        [JsonRequired]
        public string BackgroundImage { get; set; } = string.Empty;

        [JsonRequired]
        public string ImageColor { get; set; } = string.Empty;

        [JsonRequired]
        public VisualVector2 Parallax { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualBackgroundDisplayMode DisplayMode { get; set; }

        [JsonRequired]
        public bool ImageSmoothing { get; set; }

        [JsonRequired]
        public bool LockRotation { get; set; }

        [JsonRequired]
        public bool Loop { get; set; }

        [JsonRequired]
        public int ScalingRatio { get; set; }
    }

    internal sealed class CustomBackgroundEventUpdate : OffsetEventUpdate
    {
        [McpOptional]
        public string? Color { get; set; }

        [McpOptional]
        public string? BackgroundImage { get; set; }

        [McpOptional]
        public string? ImageColor { get; set; }

        [McpOptional]
        public VisualVector2? Parallax { get; set; }

        [McpOptional]
        public VisualBackgroundDisplayMode? DisplayMode { get; set; }

        [McpOptional]
        public bool? ImageSmoothing { get; set; }

        [McpOptional]
        public bool? LockRotation { get; set; }

        [McpOptional]
        public bool? Loop { get; set; }

        [McpOptional]
        public int? ScalingRatio { get; set; }
    }
}
