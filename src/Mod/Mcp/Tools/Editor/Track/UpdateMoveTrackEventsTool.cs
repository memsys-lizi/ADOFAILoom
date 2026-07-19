using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateMoveTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public UpdateMoveTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "update_move_track_events",
            Description = "Update strictly referenced MoveTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            MoveTrackEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateMoveAsync(expectedRevision, events, cancellationToken);
    }
}
