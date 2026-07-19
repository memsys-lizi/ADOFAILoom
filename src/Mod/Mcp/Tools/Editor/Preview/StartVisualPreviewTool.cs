using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.EditorPreview;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class StartVisualPreviewTool
    {
        private readonly EditorPreviewActions actions;

        public StartVisualPreviewTool(EditorPreviewActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "start_visual_preview",
            Description = "Start the editor's native level preview from an exact zero-based floor.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<EditorPreviewResult> StartVisualPreview(
            [Description("Exact current editor revision.")] string expectedRevision,
            [Description("Zero-based floor from which native editor preview starts.")] int floor,
            CancellationToken cancellationToken
        )
        {
            return actions.StartAsync(expectedRevision, floor, cancellationToken);
        }
    }
}
