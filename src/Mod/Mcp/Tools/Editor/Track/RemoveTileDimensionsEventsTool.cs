using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveTileDimensionsEventsTool
    {
        private readonly TrackEventActions actions;

        public RemoveTileDimensionsEventsTool(TrackEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_tile_dimensions_events",
            Description = "Remove strictly referenced TileDimensions events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveDimensionsAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
