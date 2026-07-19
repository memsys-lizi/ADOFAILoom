using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateTextDecorationsTool
    {
        private readonly TextEventActions actions;

        public UpdateTextDecorationsTool(TextEventActions actions) => this.actions = actions;

        [McpTool(
            "update_text_decorations",
            Description = "Update strictly referenced text decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            AddTextEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateTextDecorationsAsync(expectedRevision, events, cancellationToken);
    }
}
