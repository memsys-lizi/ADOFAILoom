using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddEmitParticleEventsTool
    {
        private readonly EmitParticleEventActions actions;

        public AddEmitParticleEventsTool(EmitParticleEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "add_emit_particle_events",
            Description = "Add typed EmitParticle events for tagged particle decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            EmitParticleEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
