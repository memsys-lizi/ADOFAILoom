using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateColorTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public UpdateColorTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "update_color_track_events",
            Description = "Update strictly referenced ColorTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ColorTrackEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateColorAsync(expectedRevision, events, cancellationToken);
    }
}
