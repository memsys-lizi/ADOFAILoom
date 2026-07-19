using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateImageDecorationsTool
    {
        private readonly ImageDecorationActions actions;

        public UpdateImageDecorationsTool(ImageDecorationActions actions) => this.actions = actions;

        [McpTool(
            "update_image_decorations",
            Description = "Update strictly referenced image decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ImageDecorationUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
