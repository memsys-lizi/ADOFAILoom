using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateDefaultTextEventsTool
    {
        private readonly DefaultTextEventActions actions;

        public UpdateDefaultTextEventsTool(DefaultTextEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "update_default_text_events",
            Description = "Update strictly referenced SetDefaultText events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            DefaultTextEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
