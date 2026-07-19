using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddSetFloorIconEventsTool
    {
        private readonly TrackEventActions actions;

        public AddSetFloorIconEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "add_set_floor_icon_events",
            Description = "Add typed SetFloorIcon events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetFloorIconEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddIconAsync(expectedRevision, events, cancellationToken);
    }
}
