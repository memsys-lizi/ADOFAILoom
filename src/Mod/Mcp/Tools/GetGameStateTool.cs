using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.State;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class GetGameStateTool : IMcpTool
    {
        public const string ToolName = "get_game_state";

        private static readonly object ToolDefinition = new
        {
            name = ToolName,
            title = "Get ADOFAI game state",
            description = "Gets the current A Dance of Fire and Ice scene, mode, and pause state.",
            inputSchema = new
            {
                type = "object",
                properties = new { },
                additionalProperties = false
            },
            outputSchema = new
            {
                type = "object",
                properties = new
                {
                    schemaVersion = new { type = "integer", description = "Game-state schema version." },
                    connected = new { type = "boolean", description = "Whether the game Mod answered." },
                    scene = new { type = new[] { "string", "null" } },
                    mode = new
                    {
                        type = new[] { "string", "null" },
                        @enum = new object?[]
                        {
                            GameModes.Menu,
                            GameModes.LevelSelect,
                            GameModes.Editor,
                            GameModes.Gameplay,
                            GameModes.Unknown,
                            null
                        }
                    },
                    paused = new { type = new[] { "boolean", "null" } }
                },
                required = new[] { "schemaVersion", "connected", "scene", "mode", "paused" },
                additionalProperties = false
            },
            annotations = new
            {
                title = "Get ADOFAI game state",
                readOnlyHint = true,
                destructiveHint = false,
                idempotentHint = true,
                openWorldHint = false
            }
        };

        private readonly MainThreadDispatcher dispatcher;

        public GetGameStateTool(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public string Name => ToolName;

        public object Definition => ToolDefinition;

        public async Task<McpToolCallResult> ExecuteAsync(
            JsonElement? arguments,
            CancellationToken cancellationToken)
        {
            if (HasArguments(arguments))
            {
                return McpToolCallResult.InvalidParameters(
                    $"{ToolName} does not accept arguments.");
            }

            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                timeout.CancelAfter(McpConstants.ToolTimeoutMilliseconds);
                try
                {
                    GameState state = await dispatcher
                        .InvokeAsync(GameStateSnapshotProvider.Capture, timeout.Token)
                        .ConfigureAwait(false);
                    return McpToolCallResult.Success(state, McpJson.Serialize(state));
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException)
                {
                    return McpToolCallResult.ExecutionError(
                        "The game main thread did not answer within 5 seconds.");
                }
                catch (Exception exception)
                {
                    return McpToolCallResult.ExecutionError(
                        $"Unable to read game state: {exception.Message}");
                }
            }
        }

        private static bool HasArguments(JsonElement? arguments)
        {
            if (!arguments.HasValue)
            {
                return false;
            }

            return arguments.Value.ValueKind != JsonValueKind.Object
                || arguments.Value.EnumerateObject().MoveNext();
        }
    }
}
