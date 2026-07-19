using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateSetFloorIconEventsTool
    {
        private readonly TrackEventActions actions;

        public UpdateSetFloorIconEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "update_set_floor_icon_events",
            Description = "Update strictly referenced SetFloorIcon events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetFloorIconEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateIconAsync(expectedRevision, events, cancellationToken);
    }
}
