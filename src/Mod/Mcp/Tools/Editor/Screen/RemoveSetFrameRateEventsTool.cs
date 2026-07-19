using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveSetFrameRateEventsTool
    {
        private readonly ScreenEffectActions actions;

        public RemoveSetFrameRateEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "remove_set_frame_rate_events",
            Description = "Remove strictly referenced SetFrameRate events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveSetFrameRateAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
