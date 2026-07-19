using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddCustomBackgroundEventsTool
    {
        private readonly BackgroundEventActions actions;

        public AddCustomBackgroundEventsTool(BackgroundEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "add_custom_background_events",
            Description = "Add typed CustomBackground events and validate referenced level assets.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            CustomBackgroundEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
