using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddMoveDecorationEventsTool
    {
        private readonly MoveDecorationEventActions actions;

        public AddMoveDecorationEventsTool(MoveDecorationEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "add_move_decoration_events",
            Description = "Add typed MoveDecorations events targeting exact decoration tags.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            MoveDecorationEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
