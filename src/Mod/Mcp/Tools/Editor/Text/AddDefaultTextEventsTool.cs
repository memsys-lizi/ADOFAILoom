using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddDefaultTextEventsTool
    {
        private readonly DefaultTextEventActions actions;

        public AddDefaultTextEventsTool(DefaultTextEventActions actions) => this.actions = actions;

        [McpTool(
            "add_default_text_events",
            Description = "Add typed SetDefaultText events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            DefaultTextEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
