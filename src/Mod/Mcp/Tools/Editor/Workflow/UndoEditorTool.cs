using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.EditorWorkflow;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UndoEditorTool
    {
        private readonly EditorWorkflowActions actions;

        public UndoEditorTool(EditorWorkflowActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "undo_editor",
            Description = "Undo the latest level editor action after checking the exact level revision.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<EditorHistoryResult> UndoEditor(
            [Description("Exact current editor revision.")] string expectedRevision,
            CancellationToken cancellationToken
        )
        {
            return actions.UndoAsync(expectedRevision, cancellationToken);
        }
    }
}
