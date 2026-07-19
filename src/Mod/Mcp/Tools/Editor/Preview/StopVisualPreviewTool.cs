using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.EditorPreview;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class StopVisualPreviewTool
    {
        private readonly EditorPreviewActions actions;

        public StopVisualPreviewTool(EditorPreviewActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "stop_visual_preview",
            Description = "Stop the editor's active native level preview and return to edit mode.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<EditorPreviewResult> StopVisualPreview(CancellationToken cancellationToken)
        {
            return actions.StopAsync(cancellationToken);
        }
    }
}
