using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Actions.VisualEvents
{
    internal enum DefaultTextProperty
    {
        DefaultTextColor,
        DefaultTextShadowColor,
        LevelTitlePosition,
        LevelTitleText,
        CongratsText,
        PerfectText,
    }

    internal sealed class DefaultTextEventCreate : TweenEventCreate
    {
        [McpOptional]
        public string? DefaultTextColor { get; set; }

        [McpOptional]
        public string? DefaultTextShadowColor { get; set; }

        [McpOptional]
        public VisualVector2? LevelTitlePosition { get; set; }

        [McpOptional]
        public string? LevelTitleText { get; set; }

        [McpOptional]
        public string? CongratsText { get; set; }

        [McpOptional]
        public string? PerfectText { get; set; }
    }

    internal sealed class DefaultTextEventUpdate : TweenEventUpdate
    {
        [McpOptional]
        public string? DefaultTextColor { get; set; }

        [McpOptional]
        public string? DefaultTextShadowColor { get; set; }

        [McpOptional]
        public VisualVector2? LevelTitlePosition { get; set; }

        [McpOptional]
        public string? LevelTitleText { get; set; }

        [McpOptional]
        public string? CongratsText { get; set; }

        [McpOptional]
        public string? PerfectText { get; set; }

        [McpOptional]
        public DefaultTextProperty[]? DisabledProperties { get; set; }
    }
}
