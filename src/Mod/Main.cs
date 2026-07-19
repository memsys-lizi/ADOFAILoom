using System;
using ADOFAILoom.Mcp;
using ADOFAILoom.Threading;
using UnityModManagerNet;

namespace ADOFAILoom
{
    public static class Main
    {
        private static readonly MainThreadDispatcher Dispatcher = new MainThreadDispatcher();
        private static McpServer? server;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnUnload = OnUnload;
            modEntry.Logger.Log("ADOFAILoom loaded / ADOFAILoom 已加载");
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
                if (server != null)
                {
                    throw new InvalidOperationException("The MCP server is already enabled.");
                }

                server = McpServerFactory.Create(
                    Dispatcher,
                    message => modEntry.Logger.Log(message));
                server.Start();
                modEntry.Logger.Log("MCP server listening at http://127.0.0.1:39473/mcp");
                return true;
            }
            catch (Exception exception)
            {
                StopServer();
                modEntry.Logger.Error($"Unable to enable MCP server: {exception}");
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
            return true;
        }

        private static void StopServer()
        {
            server?.Dispose();
            server = null;
            Dispatcher.CancelPending();
        }
    }
}
