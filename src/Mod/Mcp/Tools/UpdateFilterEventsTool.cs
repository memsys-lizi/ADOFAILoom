using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateFilterEventsTool
    {
        private readonly FilterEventActions actions;

        public UpdateFilterEventsTool(FilterEventActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "update_filter_events",
            Description = "Update an ordered batch of strictly referenced standard SetFilter events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false)]
        public Task<VisualEventMutationResult> UpdateFilterEvents(
            [Description("Exact revision returned by get_editor_state or list_visual_events.")]
            string expectedRevision,
            [Description("One through 256 standard filter event updates.")]
            FilterEventUpdate[] events,
            CancellationToken cancellationToken)
        {
            return actions.UpdateAsync(expectedRevision, events, cancellationToken);
        }
    }
}
