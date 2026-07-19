using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateSetObjectEventsTool
    {
        private readonly SetObjectEventActions actions;

        public UpdateSetObjectEventsTool(SetObjectEventActions actions) => this.actions = actions;

        [McpTool(
            "update_set_object_events",
            Description = "Update strictly referenced SetObject events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetObjectEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
