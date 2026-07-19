using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ADOFAILoom.Mcp;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.Settings;
using ADOFAILoom.Threading;
using UnityModManagerNet;

namespace ADOFAILoom
{
    public static class Main
    {
        private static readonly MainThreadDispatcher Dispatcher = new MainThreadDispatcher();
        private static McpServer? server;
        private static ToolSettings? settings;
        private static McpToolAvailability? toolAvailability;
        private static ToolSettingsPanel? toolSettingsPanel;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                IReadOnlyList<McpToolPresentation> presentations =
                    McpToolPresentationCatalog.Discover(Assembly.GetExecutingAssembly());
                settings = ToolSettings.LoadStrict(modEntry);
                toolAvailability = new McpToolAvailability(
                    presentations.Select(tool => tool.Name),
                    settings.DisabledTools
                );
                toolSettingsPanel = new ToolSettingsPanel(presentations, toolAvailability);

                modEntry.OnToggle = OnToggle;
                modEntry.OnUpdate = OnUpdate;
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = OnSaveGUI;
                modEntry.OnUnload = OnUnload;
                modEntry.Logger.Log(
                    $"ADOFAILoom loaded with {presentations.Count} configurable MCP tools."
                );
                return true;
            }
            catch (Exception exception)
            {
                settings = null;
                toolAvailability = null;
                toolSettingsPanel = null;
                modEntry.Logger.Error($"Unable to load ADOFAILoom settings: {exception}");
                return false;
            }
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
                    toolAvailability
                        ?? throw new InvalidOperationException(
                            "The MCP tool availability service is not initialized."
                        ),
                    message => modEntry.Logger.Log(message)
                );
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

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            ToolSettingsPanel panel =
                toolSettingsPanel
                ?? throw new InvalidOperationException(
                    "The ADOFAILoom settings panel is not initialized."
                );
            panel.Draw();
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            SaveSettings(modEntry);
        }

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            try
            {
                SaveSettings(modEntry);
                return true;
            }
            catch (Exception exception)
            {
                modEntry.Logger.Error($"Unable to save ADOFAILoom settings: {exception}");
                return false;
            }
            finally
            {
                StopServer();
            }
        }

        private static void SaveSettings(UnityModManager.ModEntry modEntry)
        {
            ToolSettings currentSettings =
                settings
                ?? throw new InvalidOperationException(
                    "The ADOFAILoom settings instance is not initialized."
                );
            McpToolAvailability availability =
                toolAvailability
                ?? throw new InvalidOperationException(
                    "The MCP tool availability service is not initialized."
                );
            currentSettings.DisabledTools = new List<string>(availability.GetDisabledToolNames());
            currentSettings.Save(modEntry);
        }

        private static void StopServer()
        {
            server?.Dispose();
            server = null;
            Dispatcher.CancelPending();
        }
    }
}
