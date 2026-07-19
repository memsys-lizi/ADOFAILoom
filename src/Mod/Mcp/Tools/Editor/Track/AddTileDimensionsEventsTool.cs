using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddTileDimensionsEventsTool
    {
        private readonly TrackEventActions actions;

        public AddTileDimensionsEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "add_tile_dimensions_events",
            Description = "Add typed TileDimensions events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            TileDimensionsEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddDimensionsAsync(expectedRevision, events, cancellationToken);
    }
}
