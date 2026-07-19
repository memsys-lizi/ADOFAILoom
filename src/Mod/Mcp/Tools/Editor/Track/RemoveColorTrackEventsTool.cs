using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveColorTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public RemoveColorTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_color_track_events",
            Description = "Remove strictly referenced ColorTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveColorAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
