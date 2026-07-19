using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateObjectDecorationsTool
    {
        private readonly ObjectDecorationActions actions;

        public UpdateObjectDecorationsTool(ObjectDecorationActions actions) =>
            this.actions = actions;

        [McpTool(
            "update_object_decorations",
            Description = "Update strictly referenced object decorations without changing their object type.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ObjectDecorationUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
