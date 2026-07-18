using ADOFAILoom.Mcp;
using ADOFAILoom.Threading;
using UnityModManagerNet;

namespace ADOFAILoom
{
    public static class Main
    {
        private static readonly MainThreadDispatcher Dispatcher = new MainThreadDispatcher();
        private static McpHttpServer? mcpServer;

        public static UnityModManager.ModEntry? Mod { get; private set; }

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Mod = modEntry;
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnUnload = OnUnload;
            modEntry.Logger.Log("ADOFAILoom Streamable HTTP MCP mod loaded / MCP Mod 已加载");
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool enabled)
        {
            if (!enabled)
            {
                StopServer();
                modEntry.Logger.Log("MCP server disabled / MCP 服务已关闭");
                return true;
            }

            try
            {
                mcpServer ??= McpServerFactory.Create(
                    Dispatcher,
                    message => modEntry.Logger.Log(message));
                mcpServer.Start();
                modEntry.Logger.Log($"MCP server listening at {McpConstants.EndpointUrl}");
                return true;
            }
            catch (System.Exception exception)
            {
                StopServer();
                modEntry.Logger.Error($"Unable to start MCP server: {exception}");
                return false;
            }
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float deltaTime)
        {
            Dispatcher.ProcessPending();
        }

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            StopServer();
            Mod = null;
            return true;
        }

        private static void StopServer()
        {
            mcpServer?.Dispose();
            mcpServer = null;
            Dispatcher.CancelPending();
        }
    }
}
