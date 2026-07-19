using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdatePositionTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public UpdatePositionTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "update_position_track_events",
            Description = "Update strictly referenced PositionTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            PositionTrackEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdatePositionAsync(expectedRevision, events, cancellationToken);
    }
}
