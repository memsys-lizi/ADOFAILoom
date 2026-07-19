using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveHallOfMirrorsEventsTool
    {
        private readonly ScreenEffectActions actions;

        public RemoveHallOfMirrorsEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "remove_hall_of_mirrors_events",
            Description = "Remove strictly referenced HallOfMirrors events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        ) => actions.RemoveHallOfMirrorsAsync(expectedRevision, eventRefs, cancellationToken);
    }
}
