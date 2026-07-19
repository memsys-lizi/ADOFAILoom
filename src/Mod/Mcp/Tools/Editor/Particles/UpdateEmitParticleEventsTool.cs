using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateEmitParticleEventsTool
    {
        private readonly EmitParticleEventActions actions;

        public UpdateEmitParticleEventsTool(EmitParticleEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "update_emit_particle_events",
            Description = "Update strictly referenced EmitParticle events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            EmitParticleEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
