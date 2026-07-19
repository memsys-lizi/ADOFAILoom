using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddImageDecorationsTool
    {
        private readonly ImageDecorationActions actions;

        public AddImageDecorationsTool(ImageDecorationActions actions) => this.actions = actions;

        [McpTool(
            "add_image_decorations",
            Description = "Add typed image decorations from assets in the current level directory.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ImageDecorationCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
