using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum MoveDecorationProperty
    {
        Visible,
        RelativeTo,
        Image,
        PositionOffset,
        PivotOffset,
        RotationOffset,
        Scale,
        Color,
        Opacity,
        Depth,
        Parallax,
        ParallaxOffset,
        MaskingType,
        MaskingTarget,
        UseMaskingDepth,
        MaskingFrontDepth,
        MaskingBackDepth,
    }

    internal sealed class MoveDecorationEventCreate : TweenEventCreate
    {
        [JsonRequired]
        public string Tag { get; set; } = string.Empty;

        [McpOptional]
        public bool? Visible { get; set; }

        [McpOptional]
        public VisualDecorationPlacement? RelativeTo { get; set; }

        [McpOptional]
        public string? Image { get; set; }

        [McpOptional]
        public VisualVector2? PositionOffset { get; set; }

        [McpOptional]
        public VisualVector2? PivotOffset { get; set; }

        [McpOptional]
        public float? RotationOffset { get; set; }

        [McpOptional]
        public VisualVector2? Scale { get; set; }

        [McpOptional]
        public string? Color { get; set; }

        [McpOptional]
        public float? Opacity { get; set; }

        [McpOptional]
        public int? Depth { get; set; }

        [McpOptional]
        public VisualVector2? Parallax { get; set; }

        [McpOptional]
        public VisualVector2? ParallaxOffset { get; set; }

        [McpOptional]
        public VisualMaskingType? MaskingType { get; set; }

        [McpOptional]
        public string? MaskingTarget { get; set; }

        [McpOptional]
        public bool? UseMaskingDepth { get; set; }

        [McpOptional]
        public int? MaskingFrontDepth { get; set; }

        [McpOptional]
        public int? MaskingBackDepth { get; set; }
    }

    internal sealed class MoveDecorationEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public string? Tag { get; set; }

        [McpOptional]
        public bool? Visible { get; set; }

        [McpOptional]
        public VisualDecorationPlacement? RelativeTo { get; set; }

        [McpOptional]
        public string? Image { get; set; }

        [McpOptional]
        public VisualVector2? PositionOffset { get; set; }

        [McpOptional]
        public VisualVector2? PivotOffset { get; set; }

        [McpOptional]
        public float? RotationOffset { get; set; }

        [McpOptional]
        public VisualVector2? Scale { get; set; }

        [McpOptional]
        public string? Color { get; set; }

        [McpOptional]
        public float? Opacity { get; set; }

        [McpOptional]
        public int? Depth { get; set; }

        [McpOptional]
        public VisualVector2? Parallax { get; set; }

        [McpOptional]
        public VisualVector2? ParallaxOffset { get; set; }

        [McpOptional]
        public VisualMaskingType? MaskingType { get; set; }

        [McpOptional]
        public string? MaskingTarget { get; set; }

        [McpOptional]
        public bool? UseMaskingDepth { get; set; }

        [McpOptional]
        public int? MaskingFrontDepth { get; set; }

        [McpOptional]
        public int? MaskingBackDepth { get; set; }

        [McpOptional]
        public MoveDecorationProperty[]? DisabledProperties { get; set; }
    }
}
