using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualFlashPlane
    {
        Foreground,
        Background,
    }

    internal sealed class FlashEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public VisualFlashPlane Plane { get; set; }

        [JsonRequired]
        public string StartColor { get; set; } = string.Empty;

        [JsonRequired]
        public float StartOpacity { get; set; }

        [JsonRequired]
        public string EndColor { get; set; } = string.Empty;

        [JsonRequired]
        public float EndOpacity { get; set; }
    }

    internal sealed class FlashEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public VisualFlashPlane? Plane { get; set; }

        [McpOptional]
        public string? StartColor { get; set; }

        [McpOptional]
        public float? StartOpacity { get; set; }

        [McpOptional]
        public string? EndColor { get; set; }

        [McpOptional]
        public float? EndOpacity { get; set; }
    }

    internal sealed class BloomEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public bool Enabled { get; set; }

        [JsonRequired]
        public float Threshold { get; set; }

        [JsonRequired]
        public float Intensity { get; set; }

        [JsonRequired]
        public string Color { get; set; } = string.Empty;
    }

    internal sealed class BloomEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public bool? Enabled { get; set; }

        [McpOptional]
        public float? Threshold { get; set; }

        [McpOptional]
        public float? Intensity { get; set; }

        [McpOptional]
        public string? Color { get; set; }
    }

    internal sealed class ShakeScreenEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public float Strength { get; set; }

        [JsonRequired]
        public float Intensity { get; set; }

        [JsonRequired]
        public bool FadeOut { get; set; }
    }

    internal sealed class ShakeScreenEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public float? Strength { get; set; }

        [McpOptional]
        public float? Intensity { get; set; }

        [McpOptional]
        public bool? FadeOut { get; set; }
    }

    internal sealed class ScreenTileEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public VisualVector2 Tile { get; set; } = new VisualVector2();
    }

    internal sealed class ScreenTileEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public VisualVector2? Tile { get; set; }
    }

    internal sealed class ScreenScrollEventCreate : OffsetEventCreate
    {
        [JsonRequired]
        public VisualVector2 Scroll { get; set; } = new VisualVector2();
    }

    internal sealed class ScreenScrollEventUpdate : OffsetEventUpdate
    {
        [McpOptional]
        public VisualVector2? Scroll { get; set; }
    }

    internal sealed class HallOfMirrorsEventCreate : OffsetEventCreate
    {
        [JsonRequired]
        public bool Enabled { get; set; }
    }

    internal sealed class HallOfMirrorsEventUpdate : OffsetEventUpdate
    {
        [McpOptional]
        public bool? Enabled { get; set; }
    }

    internal sealed class SetFrameRateEventCreate : OffsetEventCreate
    {
        [JsonRequired]
        public bool Enabled { get; set; }

        [JsonRequired]
        public float FrameRate { get; set; }
    }

    internal sealed class SetFrameRateEventUpdate : OffsetEventUpdate
    {
        [McpOptional]
        public bool? Enabled { get; set; }

        [McpOptional]
        public float? FrameRate { get; set; }
    }
}
