using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveFlashEventsTool
    {
        private readonly ScreenEffectActions actions;

        public RemoveFlashEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "remove_flash_events",
            Description = "Remove strictly referenced Flash events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveFlashAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
