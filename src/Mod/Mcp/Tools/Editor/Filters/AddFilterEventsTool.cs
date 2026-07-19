using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddFilterEventsTool
    {
        private readonly FilterEventActions actions;

        public AddFilterEventsTool(FilterEventActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "add_filter_events",
            Description = "Add an ordered batch of standard SetFilter events using native editor units.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> AddFilterEvents(
            [Description("Exact revision returned by get_editor_state or list_visual_events.")]
                string expectedRevision,
            [Description("One through 256 standard filter events, preserved in request order.")]
                FilterEventCreate[] events,
            CancellationToken cancellationToken
        )
        {
            return actions.AddAsync(expectedRevision, events, cancellationToken);
        }
    }
}
