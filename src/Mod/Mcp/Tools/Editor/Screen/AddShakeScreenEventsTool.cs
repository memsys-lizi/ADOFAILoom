using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddShakeScreenEventsTool
    {
        private readonly ScreenEffectActions actions;

        public AddShakeScreenEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "add_shake_screen_events",
            Description = "Add an ordered batch of ShakeScreen events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ShakeScreenEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddShakeScreenAsync(expectedRevision, events, cancellationToken);
    }
}
