using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateHallOfMirrorsEventsTool
    {
        private readonly ScreenEffectActions actions;

        public UpdateHallOfMirrorsEventsTool(ScreenEffectActions actions) => this.actions = actions;

        [McpTool(
            "update_hall_of_mirrors_events",
            Description = "Update strictly referenced HallOfMirrors events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> Execute(
            string expectedRevision,
            HallOfMirrorsEventUpdate[] events,
            CancellationToken cancellationToken
        ) => actions.UpdateHallOfMirrorsAsync(expectedRevision, events, cancellationToken);
    }
}
