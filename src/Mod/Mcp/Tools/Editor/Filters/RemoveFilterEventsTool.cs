using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveFilterEventsTool
    {
        private readonly FilterEventActions actions;

        public RemoveFilterEventsTool(FilterEventActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "remove_filter_events",
            Description = "Remove a batch of strictly referenced standard SetFilter events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> RemoveFilterEvents(
            [Description("Exact revision returned by get_editor_state or list_visual_events.")]
                string expectedRevision,
            [Description("One through 256 current SetFilter event references.")]
                VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        )
        {
            return actions.RemoveAsync(expectedRevision, eventRefs, cancellationToken);
        }
    }
}
