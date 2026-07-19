using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveBloomEventsTool
    {
        private readonly ScreenEffectActions actions;

        public RemoveBloomEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "remove_bloom_events",
            Description = "Remove strictly referenced Bloom events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveBloomAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
