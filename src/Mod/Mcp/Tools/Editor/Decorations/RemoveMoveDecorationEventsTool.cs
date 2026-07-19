using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveMoveDecorationEventsTool
    {
        private readonly MoveDecorationEventActions actions;

        public RemoveMoveDecorationEventsTool(MoveDecorationEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "remove_move_decoration_events",
            Description = "Remove strictly referenced MoveDecorations events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
