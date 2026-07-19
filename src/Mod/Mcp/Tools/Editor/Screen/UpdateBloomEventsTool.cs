using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateBloomEventsTool
    {
        private readonly ScreenEffectActions actions;

        public UpdateBloomEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "update_bloom_events",
            Description = "Update strictly referenced Bloom events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            BloomEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateBloomAsync(expectedRevision, events, cancellationToken);
    }
}
