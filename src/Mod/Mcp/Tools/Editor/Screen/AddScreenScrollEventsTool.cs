using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddScreenScrollEventsTool
    {
        private readonly ScreenEffectActions actions;

        public AddScreenScrollEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "add_screen_scroll_events",
            Description = "Add an ordered batch of ScreenScroll events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ScreenScrollEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddScreenScrollAsync(expectedRevision, events, cancellationToken);
    }
}
