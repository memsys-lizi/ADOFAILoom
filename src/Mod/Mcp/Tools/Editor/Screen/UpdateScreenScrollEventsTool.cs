using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateScreenScrollEventsTool
    {
        private readonly ScreenEffectActions actions;

        public UpdateScreenScrollEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "update_screen_scroll_events",
            Description = "Update strictly referenced ScreenScroll events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ScreenScrollEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateScreenScrollAsync(expectedRevision, events, cancellationToken);
    }
}
