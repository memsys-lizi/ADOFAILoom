using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualObjectDecorationType
    {
        Floor,
        Planet,
        PlayerBubble,
    }

    internal enum VisualPlanetColorType
    {
        DefaultRed,
        DefaultBlue,
        Gold,
        Overseer,
        Custom,
    }

    internal enum VisualFloorObjectType
    {
        Normal,
        Midspin,
    }

    internal enum VisualFloorColorType
    {
        Single,
        Glow,
        Blink,
        Switch,
        Rainbow,
        Volume,
    }

    internal abstract class ObjectDecorationProperties : FloorEventCreate
    {
        [JsonRequired]
        public string Tag { get; set; } = string.Empty;

        [JsonRequired]
        public VisualVector2 Position { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualDecorationPlacement RelativeTo { get; set; }

        [JsonRequired]
        public VisualVector2 PivotOffset { get; set; } = new VisualVector2();

        [JsonRequired]
        public float Rotation { get; set; }

        [JsonRequired]
        public bool LockRotation { get; set; }

        [JsonRequired]
        public VisualVector2 Scale { get; set; } = new VisualVector2();

        [JsonRequired]
        public bool LockScale { get; set; }

        [JsonRequired]
        public int Depth { get; set; }

        [JsonRequired]
        public bool SyncFloorDepth { get; set; }

        [JsonRequired]
        public VisualVector2 Parallax { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualVector2 ParallaxOffset { get; set; } = new VisualVector2();
    }

    internal sealed class ObjectDecorationCreate : ObjectDecorationProperties
    {
        [JsonRequired]
        public VisualObjectDecorationType ObjectType { get; set; }

        [McpOptional]
        public int? BubbleAppearStartOffset { get; set; }

        [McpOptional]
        public int? BubbleAppearEndOffset { get; set; }

        [McpOptional]
        public int? BubbleDisappearOffset { get; set; }

        [McpOptional]
        public int? BubbleSpawnOffset { get; set; }

        [McpOptional]
        public VisualPlanetColorType? PlanetColorType { get; set; }

        [McpOptional]
        public string? PlanetColor { get; set; }

        [McpOptional]
        public string? PlanetTailColor { get; set; }

        [McpOptional]
        public VisualFloorObjectType? TrackType { get; set; }

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
        public float? TrackSetSpeedIconBpm { get; set; }

        [McpOptional]
        public bool? TrackGlowEnabled { get; set; }

        [McpOptional]
        public string? TrackGlowColor { get; set; }

        [McpOptional]
        public bool? TrackIconOutlines { get; set; }
    }

    internal sealed class ObjectDecorationUpdate : FloorEventUpdate
    {
        [McpOptional]
        public string? Tag { get; set; }

        [McpOptional]
        public VisualVector2? Position { get; set; }

        [McpOptional]
        public VisualDecorationPlacement? RelativeTo { get; set; }

        [McpOptional]
        public VisualVector2? PivotOffset { get; set; }

        [McpOptional]
        public float? Rotation { get; set; }

        [McpOptional]
        public bool? LockRotation { get; set; }

        [McpOptional]
        public VisualVector2? Scale { get; set; }

        [McpOptional]
        public bool? LockScale { get; set; }

        [McpOptional]
        public int? Depth { get; set; }

        [McpOptional]
        public bool? SyncFloorDepth { get; set; }

        [McpOptional]
        public VisualVector2? Parallax { get; set; }

        [McpOptional]
        public VisualVector2? ParallaxOffset { get; set; }

        [McpOptional]
        public int? BubbleAppearStartOffset { get; set; }

        [McpOptional]
        public int? BubbleAppearEndOffset { get; set; }

        [McpOptional]
        public int? BubbleDisappearOffset { get; set; }

        [McpOptional]
        public int? BubbleSpawnOffset { get; set; }

        [McpOptional]
        public VisualPlanetColorType? PlanetColorType { get; set; }

        [McpOptional]
        public string? PlanetColor { get; set; }

        [McpOptional]
        public string? PlanetTailColor { get; set; }

        [McpOptional]
        public VisualFloorObjectType? TrackType { get; set; }

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
        public float? TrackSetSpeedIconBpm { get; set; }

        [McpOptional]
        public bool? TrackGlowEnabled { get; set; }

        [McpOptional]
        public string? TrackGlowColor { get; set; }

        [McpOptional]
        public bool? TrackIconOutlines { get; set; }
    }
}
