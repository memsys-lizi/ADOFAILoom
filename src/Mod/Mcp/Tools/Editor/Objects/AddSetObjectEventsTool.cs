using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddSetObjectEventsTool
    {
        private readonly SetObjectEventActions actions;

        public AddSetObjectEventsTool(SetObjectEventActions actions) => this.actions = actions;

        [McpTool(
            "add_set_object_events",
            Description = "Add typed SetObject events for tagged object decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetObjectEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
