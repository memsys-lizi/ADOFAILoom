using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddAnimateTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public AddAnimateTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "add_animate_track_events",
            Description = "Add typed AnimateTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            AnimateTrackEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAnimateAsync(expectedRevision, events, cancellationToken);
    }
}
