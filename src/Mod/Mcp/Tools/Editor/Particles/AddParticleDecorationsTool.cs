using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddParticleDecorationsTool
    {
        private readonly ParticleDecorationActions actions;

        public AddParticleDecorationsTool(ParticleDecorationActions actions) =>
            this.actions = actions;

        [McpTool(
            "add_particle_decorations",
            Description = "Add fully typed particle decorations with strict gradient and range values.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            ParticleDecorationCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddAsync(expectedRevision, events, cancellationToken);
    }
}
