using System.ComponentModel;
using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal abstract class FloorEventCreate
    {
        [JsonRequired]
        [Description("Zero-based floor index where the event starts.")]
        public int Floor { get; set; }
    }

    internal abstract class FloorEventUpdate
    {
        [JsonRequired]
        [Description("Strict reference returned by list_visual_events.")]
        public VisualEventReference Reference { get; set; } = new VisualEventReference();

        [McpOptional]
        public int? Floor { get; set; }
    }

    internal abstract class OffsetEventCreate : FloorEventCreate
    {
        [JsonRequired]
        [Description("Timing offset in degrees from the floor start.")]
        public float AngleOffset { get; set; }

        [McpOptional]
        [Description("Optional exact event tag. Omit to use the game's empty default.")]
        public string? EventTag { get; set; }
    }

    internal abstract class OffsetEventUpdate : FloorEventUpdate
    {
        [McpOptional]
        public float? AngleOffset { get; set; }

        [McpOptional]
        public string? EventTag { get; set; }
    }

    internal abstract class TweenEventCreate : OffsetEventCreate
    {
        [JsonRequired]
        [Description("Transition duration in beats.")]
        public float Duration { get; set; }

        [JsonRequired]
        [Description("Exact easing function used by the editor.")]
        public VisualEase Ease { get; set; }
    }

    internal abstract class TweenEventUpdate : OffsetEventUpdate
    {
        [McpOptional]
        public float? Duration { get; set; }

        [McpOptional]
        public VisualEase? Ease { get; set; }
    }

    internal enum VisualTileRelativeTo
    {
        ThisTile,
        Start,
        End,
    }

    internal sealed class VisualTileReference
    {
        [JsonRequired]
        [Description("Signed floor offset in the selected reference frame.")]
        public int Offset { get; set; }

        [JsonRequired]
        [Description("Exact tile reference frame.")]
        public VisualTileRelativeTo RelativeTo { get; set; }
    }
}
