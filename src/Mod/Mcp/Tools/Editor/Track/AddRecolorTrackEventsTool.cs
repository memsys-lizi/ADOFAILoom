using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddRecolorTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public AddRecolorTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "add_recolor_track_events",
            Description = "Add typed RecolorTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            RecolorTrackEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddRecolorAsync(expectedRevision, events, cancellationToken);
    }
}
