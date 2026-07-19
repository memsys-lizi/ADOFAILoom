using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualFont
    {
        Default,
        Arial,
        ComicSansMS,
        CourierNew,
        Georgia,
        Impact,
        TimesNewRoman,
    }

    internal enum VisualDecorationPlacement
    {
        Tile,
        Global,
        RedPlanet,
        BluePlanet,
        GreenPlanet,
        Camera,
        CameraAspect,
        LastPosition,
    }

    internal sealed class AddTextEventCreate : FloorEventCreate
    {
        [JsonRequired]
        public string Text { get; set; } = string.Empty;

        [JsonRequired]
        public string Tag { get; set; } = string.Empty;

        [JsonRequired]
        public VisualFont Font { get; set; }

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
        public string Color { get; set; } = string.Empty;

        [JsonRequired]
        public float Opacity { get; set; }

        [JsonRequired]
        public int Depth { get; set; }

        [JsonRequired]
        public VisualVector2 Parallax { get; set; } = new VisualVector2();

        [JsonRequired]
        public VisualVector2 ParallaxOffset { get; set; } = new VisualVector2();
    }

    internal sealed class AddTextEventUpdate : FloorEventUpdate
    {
        [McpOptional]
        public string? Text { get; set; }

        [McpOptional]
        public string? Tag { get; set; }

        [McpOptional]
        public VisualFont? Font { get; set; }

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
        public string? Color { get; set; }

        [McpOptional]
        public float? Opacity { get; set; }

        [McpOptional]
        public int? Depth { get; set; }

        [McpOptional]
        public VisualVector2? Parallax { get; set; }

        [McpOptional]
        public VisualVector2? ParallaxOffset { get; set; }
    }

    internal sealed class SetTextEventCreate : OffsetEventCreate
    {
        [JsonRequired]
        public string Text { get; set; } = string.Empty;

        [JsonRequired]
        public string Tag { get; set; } = string.Empty;
    }

    internal sealed class SetTextEventUpdate : OffsetEventUpdate
    {
        [McpOptional]
        public string? Text { get; set; }

        [McpOptional]
        public string? Tag { get; set; }
    }
}
