using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class AddHallOfMirrorsEventsTool
    {
        private readonly ScreenEffectActions actions;

        public AddHallOfMirrorsEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "add_hall_of_mirrors_events",
            Description = "Add an ordered batch of HallOfMirrors events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            HallOfMirrorsEventCreate[] events,
            CancellationToken cancellationToken
        ) => actions.AddHallOfMirrorsAsync(expectedRevision, events, cancellationToken);
    }
}
