using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddPositionTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public AddPositionTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "add_position_track_events",
            Description = "Add typed PositionTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            PositionTrackEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddPositionAsync(expectedRevision, events, cancellationToken);
    }
}
