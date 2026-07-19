using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveScreenTileEventsTool
    {
        private readonly ScreenEffectActions actions;

        public RemoveScreenTileEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "remove_screen_tile_events",
            Description = "Remove strictly referenced ScreenTile events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveScreenTileAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
