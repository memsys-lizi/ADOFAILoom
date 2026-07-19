using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddScreenTileEventsTool
    {
        private readonly ScreenEffectActions actions;

        public AddScreenTileEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "add_screen_tile_events",
            Description = "Add an ordered batch of ScreenTile events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ScreenTileEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddScreenTileAsync(expectedRevision, events, cancellationToken);
    }
}
