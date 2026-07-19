using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddFlashEventsTool
    {
        private readonly ScreenEffectActions actions;

        public AddFlashEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "add_flash_events",
            Description = "Add an ordered batch of Flash events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            [Description("Exact current editor revision.")] string expectedRevision,
            [Description("One through 256 Flash events.")] FlashEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddFlashAsync(expectedRevision, events, cancellationToken);
    }
}
