using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveSetFloorIconEventsTool
    {
        private readonly TrackEventActions actions;

        public RemoveSetFloorIconEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_set_floor_icon_events",
            Description = "Remove strictly referenced SetFloorIcon events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveIconAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
