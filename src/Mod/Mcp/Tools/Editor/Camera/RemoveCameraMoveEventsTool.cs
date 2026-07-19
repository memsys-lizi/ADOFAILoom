using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class RemoveCameraMoveEventsTool
    {
        private readonly CameraMoveEventActions actions;

        public RemoveCameraMoveEventsTool(CameraMoveEventActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "remove_camera_move_events",
            Description = "Remove a batch of strictly referenced MoveCamera events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false
        )]
        public Task<VisualEventMutationResult> RemoveCameraMoveEvents(
            [Description("Exact revision returned by get_editor_state or list_visual_events.")]
                string expectedRevision,
            [Description("One through 256 current MoveCamera event references.")]
                VisualEventReference[] eventRefs,
            CancellationToken cancellationToken
        )
        {
            return actions.RemoveAsync(expectedRevision, eventRefs, cancellationToken);
        }
    }
}
