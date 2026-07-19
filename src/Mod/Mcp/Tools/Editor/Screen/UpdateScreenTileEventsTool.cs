using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateScreenTileEventsTool
    {
        private readonly ScreenEffectActions actions;

        public UpdateScreenTileEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "update_screen_tile_events",
            Description = "Update strictly referenced ScreenTile events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ScreenTileEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateScreenTileAsync(expectedRevision, events, cancellationToken);
    }
}
