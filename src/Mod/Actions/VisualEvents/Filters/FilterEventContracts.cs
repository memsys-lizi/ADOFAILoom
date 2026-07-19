using System.ComponentModel;
using System.Text.Json.Serialization;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum VisualFilter
    {
        Grayscale,
        Sepia,
        Invert,
        VHS,
        EightiesTV,
        FiftiesTV,
        Arcade,
        LED,
        Rain,
        Blizzard,
        PixelSnow,
        Compression,
        Glitch,
        Pixelate,
        Waves,
        Static,
        Grain,
        MotionBlur,
        Fisheye,
        Aberration,
        Drawing,
        Neon,
        Handheld,
        NightVision,
        Funk,
        Tunnel,
        Weird3D,
        Blur,
        BlurFocus,
        GaussianBlur,
        HexagonBlack,
        Posterize,
        Sharpen,
        Contrast,
        EdgeBlackLine,
        OilPaint,
        SuperDot,
        WaterDrop,
        LightWater,
        Petals,
        PetalsInstant,
    }

    internal sealed class FilterEventCreate
    {
        [JsonRequired]
        [Description("Zero-based floor index where the event starts.")]
        public int Floor { get; set; }

        [JsonRequired]
        [Description("Timing offset in degrees from the floor start.")]
        public float AngleOffset { get; set; }

        [JsonRequired]
        [Description("Exact standard filter name exposed by the game.")]
        public VisualFilter Filter { get; set; }

        [JsonRequired]
        [Description("Whether the selected filter is enabled.")]
        public bool Enabled { get; set; }

        [JsonRequired]
        [Description("Filter intensity in percent.")]
        public float Intensity { get; set; }

        [JsonRequired]
        [Description("Whether other standard filters are disabled when this event runs.")]
        public bool DisableOthers { get; set; }

        [JsonRequired]
        [Description("Transition duration in beats.")]
        public float Duration { get; set; }

        [JsonRequired]
        [Description("Exact easing function used by the editor.")]
        public VisualEase Ease { get; set; }
    }

    internal sealed class FilterEventUpdate
    {
        [JsonRequired]
        [Description("Strict reference returned by list_visual_events.")]
        public VisualEventReference Reference { get; set; } = new VisualEventReference();

        [McpOptional]
        public int? Floor { get; set; }

        [McpOptional]
        public float? AngleOffset { get; set; }

        [McpOptional]
        public VisualFilter? Filter { get; set; }

        [McpOptional]
        public bool? Enabled { get; set; }

        [McpOptional]
        public float? Intensity { get; set; }

        [McpOptional]
        public bool? DisableOthers { get; set; }

        [McpOptional]
        public float? Duration { get; set; }

        [McpOptional]
        public VisualEase? Ease { get; set; }
    }
}
