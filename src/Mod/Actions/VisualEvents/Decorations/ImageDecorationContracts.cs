using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualDecorationBlendMode
    {
        None,
        Screen,
        LinearDodge,
        Overlay,
        SoftLight,
        Difference,
        Multiply,
    }

    internal enum VisualMaskingType
    {
        None,
        Mask,
        VisibleInsideMask,
        VisibleOutsideMask,
    }

    internal sealed class ImageDecorationCreate : FloorEventCreate
    {
        [JsonRequired]
        public string Image { get; set; } = string.Empty;

        [JsonRequired]
        public string Tag { get; set; } = string.Empty;

        [JsonRequired]
        public VisualVector2 Position { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualDecorationPlacement RelativeTo { get; set; }

        [JsonRequired]
        public bool StickToFloor { get; set; }

        [JsonRequired]
        public VisualVector2 PivotOffset { get; set; } = new VisualVector2();

        [JsonRequired]
        public float Rotation { get; set; }

        [JsonRequired]
        public bool LockRotation { get; set; }

        [JsonRequired]
        public VisualVector2 Scale { get; set; } = new VisualVector2();

        [JsonRequired]
        public float ScaleMultiplier { get; set; }

        [JsonRequired]
        public bool LockScale { get; set; }

        [JsonRequired]
        public VisualVector2 Tile { get; set; } = new VisualVector2();

        [JsonRequired]
        public string Color { get; set; } = string.Empty;

        [JsonRequired]
        public float Opacity { get; set; }

        [JsonRequired]
        public int Depth { get; set; }

        [JsonRequired]
        public bool SyncFloorDepth { get; set; }

        [JsonRequired]
        public VisualVector2 Parallax { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualVector2 ParallaxOffset { get; set; } = new VisualVector2();

        [JsonRequired]
        public bool ImageSmoothing { get; set; }

        [JsonRequired]
        public VisualDecorationBlendMode BlendMode { get; set; }

        [JsonRequired]
        public VisualMaskingType MaskingType { get; set; }

        [JsonRequired]
        public string MaskingTarget { get; set; } = string.Empty;

        [JsonRequired]
        public bool UseMaskingDepth { get; set; }

        [JsonRequired]
        public int MaskingFrontDepth { get; set; }

        [JsonRequired]
        public int MaskingBackDepth { get; set; }
    }

    internal sealed class ImageDecorationUpdate : FloorEventUpdate
    {
        [McpOptional]
        public string? Image { get; set; }

        [McpOptional]
        public string? Tag { get; set; }

        [McpOptional]
        public VisualVector2? Position { get; set; }

        [McpOptional]
        public VisualDecorationPlacement? RelativeTo { get; set; }

        [McpOptional]
        public bool? StickToFloor { get; set; }

        [McpOptional]
        public VisualVector2? PivotOffset { get; set; }

        [McpOptional]
        public float? Rotation { get; set; }

        [McpOptional]
        public bool? LockRotation { get; set; }

        [McpOptional]
        public VisualVector2? Scale { get; set; }

        [McpOptional]
        public float? ScaleMultiplier { get; set; }

        [McpOptional]
        public bool? LockScale { get; set; }

        [McpOptional]
        public VisualVector2? Tile { get; set; }

        [McpOptional]
        public string? Color { get; set; }

        [McpOptional]
        public float? Opacity { get; set; }

        [McpOptional]
        public int? Depth { get; set; }

        [McpOptional]
        public bool? SyncFloorDepth { get; set; }

        [McpOptional]
        public VisualVector2? Parallax { get; set; }

        [McpOptional]
        public VisualVector2? ParallaxOffset { get; set; }

        [McpOptional]
        public bool? ImageSmoothing { get; set; }

        [McpOptional]
        public VisualDecorationBlendMode? BlendMode { get; set; }

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
}
