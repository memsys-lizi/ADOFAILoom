using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class GetGameStateTool
    {
        private readonly MainThreadDispatcher dispatcher;

        public GetGameStateTool(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        [McpTool(
            "get_game_state",
            Description = "Get the current A Dance of Fire and Ice game state.",
            ReadOnly = true,
            Destructive = false,
            Idempotent = true,
            OpenWorld = false)]
        public Task<GameState> GetGameState(CancellationToken cancellationToken)
        {
            return dispatcher.InvokeAsync(
                GameStateSnapshotProvider.Capture,
                McpProtocol.MainThreadTimeout,
                cancellationToken);
        }
    }
}
