using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.EditorWorkflow;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class SaveLevelTool
    {
        private readonly EditorWorkflowActions actions;

        public SaveLevelTool(EditorWorkflowActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "save_level",
            Description = "Save the current editor level and verify the written .adofai revision.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = true,
            OpenWorld = true
        )]
        public Task<LevelSaveResult> SaveLevel(
            [Description("Exact current editor revision.")] string expectedRevision,
            CancellationToken cancellationToken
        )
        {
            return actions.SaveAsync(expectedRevision, cancellationToken);
        }
    }
}
