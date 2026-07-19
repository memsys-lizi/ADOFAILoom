using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateRecolorTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public UpdateRecolorTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "update_recolor_track_events",
            Description = "Update strictly referenced RecolorTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            RecolorTrackEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateRecolorAsync(expectedRevision, events, cancellationToken);
    }
}
