using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveSetObjectEventsTool
    {
        private readonly SetObjectEventActions actions;

        public RemoveSetObjectEventsTool(SetObjectEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_set_object_events",
            Description = "Remove strictly referenced SetObject events.",
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
