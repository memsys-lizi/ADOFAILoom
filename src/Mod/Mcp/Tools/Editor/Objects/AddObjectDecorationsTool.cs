using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddObjectDecorationsTool
    {
        private readonly ObjectDecorationActions actions;

        public AddObjectDecorationsTool(ObjectDecorationActions actions) => this.actions = actions;

        [McpTool(
            "add_object_decorations",
            Description = "Add strictly typed floor, planet, or player-bubble object decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ObjectDecorationCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
