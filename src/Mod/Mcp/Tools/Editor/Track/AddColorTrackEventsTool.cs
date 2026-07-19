using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddColorTrackEventsTool
    {
        private readonly TrackEventActions actions;

        public AddColorTrackEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "add_color_track_events",
            Description = "Add typed ColorTrack events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ColorTrackEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddColorAsync(expectedRevision, events, cancellationToken);
    }
}
