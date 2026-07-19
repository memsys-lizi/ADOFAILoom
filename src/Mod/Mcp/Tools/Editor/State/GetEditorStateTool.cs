using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class GetEditorStateTool
    {
        private readonly MainThreadDispatcher dispatcher;

        public GetEditorStateTool(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        [McpTool(
            "get_editor_state",
            Description = "Get the current level editor state and strict level revision.",
            ReadOnly = true,
            Destructive = false,
            Idempotent = true,
            OpenWorld = false
        )]
        public Task<EditorState> GetEditorState(CancellationToken cancellationToken)
        {
            return dispatcher.InvokeAsync(
                EditorStateProvider.Capture,
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }
    }
}
