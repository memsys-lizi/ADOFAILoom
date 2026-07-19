using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveMoveTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public RemoveMoveTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_move_track_events",
            Description = "Remove strictly referenced MoveTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveMoveAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
