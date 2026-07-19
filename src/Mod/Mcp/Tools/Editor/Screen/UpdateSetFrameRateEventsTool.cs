using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateSetFrameRateEventsTool
    {
        private readonly ScreenEffectActions actions;

        public UpdateSetFrameRateEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "update_set_frame_rate_events",
            Description = "Update strictly referenced SetFrameRate events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetFrameRateEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateSetFrameRateAsync(expectedRevision, events, cancellationToken);
    }
}
