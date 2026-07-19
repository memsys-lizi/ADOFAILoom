using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateAnimateTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public UpdateAnimateTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "update_animate_track_events",
            Description = "Update strictly referenced AnimateTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            AnimateTrackEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAnimateAsync(expectedRevision, events, cancellationToken);
    }
}
