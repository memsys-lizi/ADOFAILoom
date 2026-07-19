using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateMoveDecorationEventsTool
    {
        private readonly MoveDecorationEventActions actions;

        public UpdateMoveDecorationEventsTool(MoveDecorationEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "update_move_decoration_events",
            Description = "Update strictly referenced MoveDecorations events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            MoveDecorationEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
