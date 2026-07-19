using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveAnimateTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public RemoveAnimateTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_animate_track_events",
            Description = "Remove strictly referenced AnimateTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveAnimateAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
