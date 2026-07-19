using System.ComponentModel;
using System.Text.Json;
using ADOFAILoom.McpServer.Bridge;
using ADOFAILoom.Protocol;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace ADOFAILoom.McpServer.Tools;

[McpServerToolType]
public static class GameStateTools
{
    [McpServerTool(
        Name = BridgeProtocol.GetGameStateMethod,
        Title = "Get ADOFAI game state",
        ReadOnly = true,
        Destructive = false,
        Idempotent = true,
        OpenWorld = false,
        UseStructuredContent = true,
        OutputSchemaType = typeof(GameState))]
    [Description("Gets the current A Dance of Fire and Ice scene, mode, and pause state.")]
    public static async Task<CallToolResult> GetGameStateAsync(
        GameBridgeClient bridgeClient,
        CancellationToken cancellationToken)
    {
        try
        {
            GameState state = await bridgeClient
                .GetGameStateAsync(cancellationToken)
                .ConfigureAwait(false);
            string json = JsonSerializer.Serialize(state, PipeMessageIO.JsonOptions);

            return new CallToolResult
            {
                Content = new List<ContentBlock>
                {
                    new TextContentBlock { Text = json }
                },
                StructuredContent = JsonSerializer.SerializeToElement(
                    state,
                    PipeMessageIO.JsonOptions),
                IsError = false
            };
        }
        catch (McpException exception)
        {
            return new CallToolResult
            {
                Content = new List<ContentBlock>
                {
                    new TextContentBlock { Text = exception.Message }
                },
                IsError = true
            };
        }
    }
}
