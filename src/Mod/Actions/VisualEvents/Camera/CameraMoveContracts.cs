using System.ComponentModel;
using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum CameraRelativeTo
    {
        Player,
        Tile,
        Global,
        LastPosition,
        LastPositionNoRotation,
    }

    internal enum CameraMoveProperty
    {
        Position,
        Rotation,
        Zoom,
        RelativeTo,
    }

    internal sealed class CameraMoveCreate
    {
        [JsonRequired]
        [Description("Zero-based floor index where the event starts.")]
        public int Floor { get; set; }

        [JsonRequired]
        [Description("Timing offset in degrees from the floor start.")]
        public float AngleOffset { get; set; }

        [JsonRequired]
        [Description("Transition duration in beats.")]
        public float Duration { get; set; }

        [JsonRequired]
        [Description("Exact easing function used by the editor.")]
        public VisualEase Ease { get; set; }

        [McpOptional]
        [Description("Target camera position in tile units. Omit to keep position disabled.")]
        public VisualVector2? Position { get; set; }

        [McpOptional]
        [Description("Target camera rotation in degrees. Omit to keep rotation disabled.")]
        public float? Rotation { get; set; }

        [McpOptional]
        [Description("Target camera zoom in percent. Omit to keep zoom disabled.")]
        public float? Zoom { get; set; }

        [McpOptional]
        [Description("Camera reference frame. Omit to keep relativeTo disabled.")]
        public CameraRelativeTo? RelativeTo { get; set; }
    }

    internal sealed class CameraMoveUpdate
    {
        [JsonRequired]
        [Description("Strict reference returned by list_visual_events.")]
        public VisualEventReference Reference { get; set; } = new VisualEventReference();

        [McpOptional]
        public int? Floor { get; set; }

        [McpOptional]
        public float? AngleOffset { get; set; }

        [McpOptional]
        public float? Duration { get; set; }

        [McpOptional]
        public VisualEase? Ease { get; set; }

        [McpOptional]
        public VisualVector2? Position { get; set; }

        [McpOptional]
        public float? Rotation { get; set; }

        [McpOptional]
        public float? Zoom { get; set; }

        [McpOptional]
        public CameraRelativeTo? RelativeTo { get; set; }

        [McpOptional]
        [Description("Optional camera properties to disable explicitly.")]
        public CameraMoveProperty[]? DisabledProperties { get; set; }
    }
}
