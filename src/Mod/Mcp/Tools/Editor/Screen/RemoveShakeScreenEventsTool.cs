using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveShakeScreenEventsTool
    {
        private readonly ScreenEffectActions actions;

        public RemoveShakeScreenEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "remove_shake_screen_events",
            Description = "Remove strictly referenced ShakeScreen events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveShakeScreenAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
