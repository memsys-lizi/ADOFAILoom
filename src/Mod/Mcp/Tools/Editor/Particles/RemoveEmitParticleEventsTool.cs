using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveEmitParticleEventsTool
    {
        private readonly EmitParticleEventActions actions;

        public RemoveEmitParticleEventsTool(EmitParticleEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "remove_emit_particle_events",
            Description = "Remove strictly referenced EmitParticle events.",
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
