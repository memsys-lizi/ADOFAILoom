using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemovePositionTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public RemovePositionTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_position_track_events",
            Description = "Remove strictly referenced PositionTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemovePositionAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
