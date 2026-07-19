using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddMoveTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public AddMoveTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "add_move_track_events",
            Description = "Add typed MoveTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            MoveTrackEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddMoveAsync(expectedRevision, events, cancellationToken);
    }
}
