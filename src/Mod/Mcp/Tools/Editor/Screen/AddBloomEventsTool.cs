using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddBloomEventsTool
    {
        private readonly ScreenEffectActions actions;

        public AddBloomEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "add_bloom_events",
            Description = "Add an ordered batch of Bloom events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            BloomEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddBloomAsync(expectedRevision, events, cancellationToken);
    }
}
