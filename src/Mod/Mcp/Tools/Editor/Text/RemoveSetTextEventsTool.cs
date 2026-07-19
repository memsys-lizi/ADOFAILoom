using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveSetTextEventsTool
    {
        private readonly TextEventActions actions;

        public RemoveSetTextEventsTool(TextEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_set_text_events",
            Description = "Remove strictly referenced SetText events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveSetTextAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
