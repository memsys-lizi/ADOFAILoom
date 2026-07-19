using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveSetParticleEventsTool
    {
        private readonly SetParticleEventActions actions;

        public RemoveSetParticleEventsTool(SetParticleEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "remove_set_particle_events",
            Description = "Remove strictly referenced SetParticle events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
