using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateSetTextEventsTool
    {
        private readonly TextEventActions actions;

        public UpdateSetTextEventsTool(TextEventActions actions) => this.actions = actions;

        [McpTool(
            "update_set_text_events",
            Description = "Update strictly referenced SetText events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetTextEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateSetTextAsync(expectedRevision, events, cancellationToken);
    }
}
