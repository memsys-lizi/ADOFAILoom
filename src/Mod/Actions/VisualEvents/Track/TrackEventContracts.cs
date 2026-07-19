using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualTrackColorType
    {
        Single,
        Stripes,
        Glow,
        Blink,
        Switch,
        Rainbow,
        Volume,
    }

    internal enum VisualTrackColorPulse
    {
        None,
        Forward,
        Backward,
    }

    internal enum VisualTrackStyle
    {
        Standard,
        Neon,
        NeonLight,
        Basic,
        Gems,
        Minimal,
    }

    internal enum VisualTrackAnimation
    {
        None,
        Assemble,
        Assemble_Far,
        Extend,
        Grow,
        Grow_Spin,
        Fade,
        Drop,
        Rise,
    }

    internal enum VisualTrackDisappearAnimation
    {
        None,
        Scatter,
        Scatter_Far,
        Retract,
        Shrink,
        Shrink_Spin,
        Fade,
    }

    internal enum VisualFloorIcon
    {
        None,
        Snail,
        DoubleSnail,
        Rabbit,
        DoubleRabbit,
        Swirl,
        Checkpoint,
        HoldArrowShort,
        HoldArrowLong,
        HoldReleaseShort,
        HoldReleaseLong,
        MultiPlanetTwo,
        MultiPlanetThreeMore,
        Portal,
    }

    internal enum MoveTrackProperty
    {
        PositionOffset,
        RotationOffset,
        Scale,
        Opacity,
    }

    internal enum PositionTrackProperty
    {
        PositionOffset,
        Rotation,
        Scale,
        Opacity,
        StickToFloors,
    }

    internal enum ColorTrackProperty
    {
        FloorIconOutlines,
    }

    internal enum AnimateTrackProperty
    {
        TrackAnimation,
        TrackDisappearAnimation,
    }

    internal sealed class ColorTrackEventCreate : FloorEventCreate
    {
        [JsonRequired]
        public VisualTrackColorType TrackColorType { get; set; }

        [JsonRequired]
        public string TrackColor { get; set; } = string.Empty;

        [JsonRequired]
        public string SecondaryTrackColor { get; set; } = string.Empty;

        [JsonRequired]
        public float TrackColorAnimationDuration { get; set; }

        [JsonRequired]
        public VisualTrackColorPulse TrackColorPulse { get; set; }

        [JsonRequired]
        public int TrackPulseLength { get; set; }

        [JsonRequired]
        public VisualTrackStyle TrackStyle { get; set; }

        [JsonRequired]
        public string TrackTexture { get; set; } = string.Empty;

        [JsonRequired]
        public float TrackTextureScale { get; set; }

        [JsonRequired]
        public float TrackGlowIntensity { get; set; }

        [McpOptional]
        public bool? FloorIconOutlines { get; set; }

        [JsonRequired]
        public bool JustThisTile { get; set; }
    }

    internal sealed class ColorTrackEventUpdate : FloorEventUpdate
    {
        [McpOptional]
        public VisualTrackColorType? TrackColorType { get; set; }

        [McpOptional]
        public string? TrackColor { get; set; }

        [McpOptional]
        public string? SecondaryTrackColor { get; set; }

        [McpOptional]
        public float? TrackColorAnimationDuration { get; set; }

        [McpOptional]
        public VisualTrackColorPulse? TrackColorPulse { get; set; }

        [McpOptional]
        public int? TrackPulseLength { get; set; }

        [McpOptional]
        public VisualTrackStyle? TrackStyle { get; set; }

        [McpOptional]
        public string? TrackTexture { get; set; }

        [McpOptional]
        public float? TrackTextureScale { get; set; }

        [McpOptional]
        public float? TrackGlowIntensity { get; set; }

        [McpOptional]
        public bool? FloorIconOutlines { get; set; }

        [McpOptional]
        public bool? JustThisTile { get; set; }

        [McpOptional]
        public ColorTrackProperty[]? DisabledProperties { get; set; }
    }

    internal sealed class AnimateTrackEventCreate : FloorEventCreate
    {
        [McpOptional]
        public VisualTrackAnimation? TrackAnimation { get; set; }

        [JsonRequired]
        public float BeatsAhead { get; set; }

        [McpOptional]
        public VisualTrackDisappearAnimation? TrackDisappearAnimation { get; set; }

        [JsonRequired]
        public float BeatsBehind { get; set; }
    }

    internal sealed class AnimateTrackEventUpdate : FloorEventUpdate
    {
        [McpOptional]
        public VisualTrackAnimation? TrackAnimation { get; set; }

        [McpOptional]
        public float? BeatsAhead { get; set; }

        [McpOptional]
        public VisualTrackDisappearAnimation? TrackDisappearAnimation { get; set; }

        [McpOptional]
        public float? BeatsBehind { get; set; }

        [McpOptional]
        public AnimateTrackProperty[]? DisabledProperties { get; set; }
    }

    internal sealed class RecolorTrackEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public VisualTileReference StartTile { get; set; } = new VisualTileReference();

        [JsonRequired]
        public VisualTileReference EndTile { get; set; } = new VisualTileReference();

        [JsonRequired]
        public int GapLength { get; set; }

        [JsonRequired]
        public VisualTrackColorType TrackColorType { get; set; }

        [JsonRequired]
        public string TrackColor { get; set; } = string.Empty;

        [JsonRequired]
        public string SecondaryTrackColor { get; set; } = string.Empty;

        [JsonRequired]
        public float TrackColorAnimationDuration { get; set; }

        [JsonRequired]
        public VisualTrackColorPulse TrackColorPulse { get; set; }

        [JsonRequired]
        public int TrackPulseLength { get; set; }

        [JsonRequired]
        public VisualTrackStyle TrackStyle { get; set; }

        [JsonRequired]
        public float TrackGlowIntensity { get; set; }
    }

    internal sealed class RecolorTrackEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public VisualTileReference? StartTile { get; set; }

        [McpOptional]
        public VisualTileReference? EndTile { get; set; }

        [McpOptional]
        public int? GapLength { get; set; }

        [McpOptional]
        public VisualTrackColorType? TrackColorType { get; set; }

        [McpOptional]
        public string? TrackColor { get; set; }

        [McpOptional]
        public string? SecondaryTrackColor { get; set; }

        [McpOptional]
        public float? TrackColorAnimationDuration { get; set; }

        [McpOptional]
        public VisualTrackColorPulse? TrackColorPulse { get; set; }

        [McpOptional]
        public int? TrackPulseLength { get; set; }

        [McpOptional]
        public VisualTrackStyle? TrackStyle { get; set; }

        [McpOptional]
        public float? TrackGlowIntensity { get; set; }
    }

    internal sealed class MoveTrackEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public VisualTileReference StartTile { get; set; } = new VisualTileReference();

        [JsonRequired]
        public VisualTileReference EndTile { get; set; } = new VisualTileReference();

        [JsonRequired]
        public int GapLength { get; set; }

        [McpOptional]
        public VisualVector2? PositionOffset { get; set; }

        [McpOptional]
        public float? RotationOffset { get; set; }

        [McpOptional]
        public VisualVector2? Scale { get; set; }

        [McpOptional]
        public float? Opacity { get; set; }

        [JsonRequired]
        public bool MaximumVfxOnly { get; set; }
    }

    internal sealed class MoveTrackEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public VisualTileReference? StartTile { get; set; }

        [McpOptional]
        public VisualTileReference? EndTile { get; set; }

        [McpOptional]
        public int? GapLength { get; set; }

        [McpOptional]
        public VisualVector2? PositionOffset { get; set; }

        [McpOptional]
        public float? RotationOffset { get; set; }

        [McpOptional]
        public VisualVector2? Scale { get; set; }

        [McpOptional]
        public float? Opacity { get; set; }

        [McpOptional]
        public bool? MaximumVfxOnly { get; set; }

        [McpOptional]
        public MoveTrackProperty[]? DisabledProperties { get; set; }
    }

    internal sealed class PositionTrackEventCreate : FloorEventCreate
    {
        [McpOptional]
        public VisualVector2? PositionOffset { get; set; }

        [JsonRequired]
        public VisualTileReference RelativeTo { get; set; } = new VisualTileReference();

        [McpOptional]
        public float? Rotation { get; set; }

        [McpOptional]
        public float? Scale { get; set; }

        [McpOptional]
        public float? Opacity { get; set; }

        [JsonRequired]
        public bool JustThisTile { get; set; }

        [JsonRequired]
        public bool EditorOnly { get; set; }

        [McpOptional]
        public bool? StickToFloors { get; set; }
    }

    internal sealed class PositionTrackEventUpdate : FloorEventUpdate
    {
        [McpOptional]
        public VisualVector2? PositionOffset { get; set; }

        [McpOptional]
        public VisualTileReference? RelativeTo { get; set; }

        [McpOptional]
        public float? Rotation { get; set; }

        [McpOptional]
        public float? Scale { get; set; }

        [McpOptional]
        public float? Opacity { get; set; }

        [McpOptional]
        public bool? JustThisTile { get; set; }

        [McpOptional]
        public bool? EditorOnly { get; set; }

        [McpOptional]
        public bool? StickToFloors { get; set; }

        [McpOptional]
        public PositionTrackProperty[]? DisabledProperties { get; set; }
    }

    internal sealed class TileDimensionsEventCreate : FloorEventCreate
    {
        [JsonRequired]
        public float Width { get; set; }

        [JsonRequired]
        public float Length { get; set; }
    }

    internal sealed class TileDimensionsEventUpdate : FloorEventUpdate
    {
        [McpOptional]
        public float? Width { get; set; }

        [McpOptional]
        public float? Length { get; set; }
    }

    internal sealed class SetFloorIconEventCreate : FloorEventCreate
    {
        [JsonRequired]
        public VisualFloorIcon Icon { get; set; }
    }

    internal sealed class SetFloorIconEventUpdate : FloorEventUpdate
    {
        [McpOptional]
        public VisualFloorIcon? Icon { get; set; }
    }
}
