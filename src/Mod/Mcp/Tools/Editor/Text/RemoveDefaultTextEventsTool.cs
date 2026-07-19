using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveDefaultTextEventsTool
    {
        private readonly DefaultTextEventActions actions;

        public RemoveDefaultTextEventsTool(DefaultTextEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "remove_default_text_events",
            Description = "Remove strictly referenced SetDefaultText events.",
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
