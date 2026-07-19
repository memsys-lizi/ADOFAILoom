using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddTextDecorationsTool
    {
        private readonly TextEventActions actions;

        public AddTextDecorationsTool(TextEventActions actions) => this.actions = actions;

        [McpTool(
            "add_text_decorations",
            Description = "Add typed text decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            AddTextEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddTextDecorationsAsync(expectedRevision, events, cancellationToken);
    }
}
