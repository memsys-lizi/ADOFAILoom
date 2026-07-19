using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateParticleDecorationsTool
    {
        private readonly ParticleDecorationActions actions;

        public UpdateParticleDecorationsTool(ParticleDecorationActions actions) =>
            this.actions = actions;

        [McpTool(
            "update_particle_decorations",
            Description = "Update strictly referenced particle decorations.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ParticleDecorationUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateAsync(expectedRevision, events, cancellationToken);
    }
}
