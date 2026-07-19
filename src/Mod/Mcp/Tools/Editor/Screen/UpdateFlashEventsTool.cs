using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateFlashEventsTool
    {
        private readonly ScreenEffectActions actions;

        public UpdateFlashEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "update_flash_events",
            Description = "Update strictly referenced Flash events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            FlashEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateFlashAsync(expectedRevision, events, cancellationToken);
    }
}
