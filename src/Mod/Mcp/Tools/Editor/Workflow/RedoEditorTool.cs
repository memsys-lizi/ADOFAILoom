using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.EditorWorkflow;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RedoEditorTool
    {
        private readonly EditorWorkflowActions actions;

        public RedoEditorTool(EditorWorkflowActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "redo_editor",
            Description = "Redo the latest level editor action after checking the exact level revision.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<EditorHistoryResult> RedoEditor(
            [Description("Exact current editor revision.")] string expectedRevision,
            CancellationToken cancellationToken
        )
        {
            return actions.RedoAsync(expectedRevision, cancellationToken);
        }
    }
}
