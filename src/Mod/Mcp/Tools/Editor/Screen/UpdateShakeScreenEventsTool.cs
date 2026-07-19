using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateShakeScreenEventsTool
    {
        private readonly ScreenEffectActions actions;

        public UpdateShakeScreenEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "update_shake_screen_events",
            Description = "Update strictly referenced ShakeScreen events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ShakeScreenEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateShakeScreenAsync(expectedRevision, events, cancellationToken);
    }
}
