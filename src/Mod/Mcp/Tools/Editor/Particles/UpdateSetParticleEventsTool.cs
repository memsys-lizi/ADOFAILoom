using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateSetParticleEventsTool
    {
        private readonly SetParticleEventActions actions;

        public UpdateSetParticleEventsTool(SetParticleEventActions actions) =>
            this.actions = actions;

        [McpTool(
            "update_set_particle_events",
            Description = "Update strictly referenced SetParticle events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetParticleEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
