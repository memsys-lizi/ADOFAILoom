using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class UpdateCameraMoveEventsTool
    {
        private readonly CameraMoveEventActions actions;

        public UpdateCameraMoveEventsTool(CameraMoveEventActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "update_camera_move_events",
            Description = "Update an ordered batch of strictly referenced MoveCamera events.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false)]
        public Task<VisualEventMutationResult> UpdateCameraMoveEvents(
            [Description("Exact revision returned by get_editor_state or list_visual_events.")]
            string expectedRevision,
            [Description("One through 256 camera event updates.")]
            CameraMoveUpdate[] events,
            CancellationToken cancellationToken)
        {
            return actions.UpdateAsync(expectedRevision, events, cancellationToken);
        }
    }
}
