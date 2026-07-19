using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddSetTextEventsTool
    {
        private readonly TextEventActions actions;

        public AddSetTextEventsTool(TextEventActions actions) => this.actions = actions;

        [McpTool(
            "add_set_text_events",
            Description = "Add typed SetText events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetTextEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddSetTextAsync(expectedRevision, events, cancellationToken);
    }
}
