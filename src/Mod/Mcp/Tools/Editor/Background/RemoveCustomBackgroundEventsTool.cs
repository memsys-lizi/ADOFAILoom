using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveCustomBackgroundEventsTool
    {
        private readonly BackgroundEventActions actions;

        public RemoveCustomBackgroundEventsTool(BackgroundEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "remove_custom_background_events",
            Description = "Remove strictly referenced CustomBackground events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
