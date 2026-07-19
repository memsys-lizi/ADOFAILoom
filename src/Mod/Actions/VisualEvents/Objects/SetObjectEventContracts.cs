using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum SetObjectProperty
    {
        PlanetColor,
        PlanetTailColor,
        TrackAngle,
        TrackColorType,
        TrackColor,
        SecondaryTrackColor,
        TrackColorAnimationDuration,
        TrackOpacity,
        TrackStyle,
        TrackIcon,
        TrackIconAngle,
        TrackIconFlipped,
        TrackRedSwirl,
        TrackGraySetSpeedIcon,
        TrackGlowEnabled,
        TrackGlowColor,
        TrackIconOutlines,
    }

    internal sealed class SetObjectEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public string Tag { get; set; } = string.Empty;

        [McpOptional]
        public string? PlanetColor { get; set; }

        [McpOptional]
        public string? PlanetTailColor { get; set; }

        [McpOptional]
        public float? TrackAngle { get; set; }

        [McpOptional]
        public VisualFloorColorType? TrackColorType { get; set; }

        [McpOptional]
        public string? TrackColor { get; set; }

        [McpOptional]
        public string? SecondaryTrackColor { get; set; }

        [McpOptional]
        public float? TrackColorAnimationDuration { get; set; }

        [McpOptional]
        public float? TrackOpacity { get; set; }

        [McpOptional]
        public VisualTrackStyle? TrackStyle { get; set; }

        [McpOptional]
        public VisualFloorIcon? TrackIcon { get; set; }

        [McpOptional]
        public float? TrackIconAngle { get; set; }

        [McpOptional]
        public bool? TrackIconFlipped { get; set; }

        [McpOptional]
        public bool? TrackRedSwirl { get; set; }

        [McpOptional]
        public bool? TrackGraySetSpeedIcon { get; set; }

        [McpOptional]
        public bool? TrackGlowEnabled { get; set; }

        [McpOptional]
        public string? TrackGlowColor { get; set; }

        [McpOptional]
        public bool? TrackIconOutlines { get; set; }
    }

    internal sealed class SetObjectEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public string? Tag { get; set; }

        [McpOptional]
        public string? PlanetColor { get; set; }

        [McpOptional]
        public string? PlanetTailColor { get; set; }

        [McpOptional]
        public float? TrackAngle { get; set; }

        [McpOptional]
        public VisualFloorColorType? TrackColorType { get; set; }

        [McpOptional]
        public string? TrackColor { get; set; }

        [McpOptional]
        public string? SecondaryTrackColor { get; set; }

        [McpOptional]
        public float? TrackColorAnimationDuration { get; set; }

        [McpOptional]
        public float? TrackOpacity { get; set; }

        [McpOptional]
        public VisualTrackStyle? TrackStyle { get; set; }

        [McpOptional]
        public VisualFloorIcon? TrackIcon { get; set; }

        [McpOptional]
        public float? TrackIconAngle { get; set; }

        [McpOptional]
        public bool? TrackIconFlipped { get; set; }

        [McpOptional]
        public bool? TrackRedSwirl { get; set; }

        [McpOptional]
        public bool? TrackGraySetSpeedIcon { get; set; }

        [McpOptional]
        public bool? TrackGlowEnabled { get; set; }

        [McpOptional]
        public string? TrackGlowColor { get; set; }

        [McpOptional]
        public bool? TrackIconOutlines { get; set; }

        [McpOptional]
        public SetObjectProperty[]? DisabledProperties { get; set; }
    }
}
