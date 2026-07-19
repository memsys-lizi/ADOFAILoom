using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveTextDecorationsTool
    {
        private readonly TextEventActions actions;

        public RemoveTextDecorationsTool(TextEventActions actions) => this.actions = actions;

        [McpTool(
            "remove_text_decorations",
            Description = "Remove strictly referenced text decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveTextDecorationsAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
