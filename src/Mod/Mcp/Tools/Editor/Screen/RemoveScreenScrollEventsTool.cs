using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveScreenScrollEventsTool
    {
        private readonly ScreenEffectActions actions;

        public RemoveScreenScrollEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "remove_screen_scroll_events",
            Description = "Remove strictly referenced ScreenScroll events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveScreenScrollAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
