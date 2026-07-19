using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddSetFrameRateEventsTool
    {
        private readonly ScreenEffectActions actions;

        public AddSetFrameRateEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "add_set_frame_rate_events",
            Description = "Add an ordered batch of SetFrameRate events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetFrameRateEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddSetFrameRateAsync(expectedRevision, events, cancellationToken);
    }
}
