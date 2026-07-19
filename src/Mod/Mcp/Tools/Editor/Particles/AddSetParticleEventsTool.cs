using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddSetParticleEventsTool
    {
        private readonly SetParticleEventActions actions;

        public AddSetParticleEventsTool(SetParticleEventActions actions) => this.actions = actions;

        [McpTool(
            "add_set_particle_events",
            Description = "Add typed SetParticle events for tagged particle decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            SetParticleEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
