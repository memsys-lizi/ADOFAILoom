using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveRecolorTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public RemoveRecolorTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_recolor_track_events",
            Description = "Remove strictly referenced RecolorTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveRecolorAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
