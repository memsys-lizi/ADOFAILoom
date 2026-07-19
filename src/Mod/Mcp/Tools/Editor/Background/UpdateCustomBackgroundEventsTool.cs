using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateCustomBackgroundEventsTool
    {
        private readonly BackgroundEventActions actions;

        public UpdateCustomBackgroundEventsTool(BackgroundEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "update_custom_background_events",
            Description = "Update strictly referenced CustomBackground events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            CustomBackgroundEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
