using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateTileDimensionsEventsTool
    {
        private readonly TrackEventActions actions;

        public UpdateTileDimensionsEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "update_tile_dimensions_events",
            Description = "Update strictly referenced TileDimensions events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            TileDimensionsEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateDimensionsAsync(expectedRevision, events, cancellationToken);
    }
}
