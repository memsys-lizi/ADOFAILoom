using System;
using System.Collections.Generic;
using ADOFAILoom.Actions;
using ADOFAILoom.Actions.EditorWorkflow;
using ADOFAILoom.Actions.VisualEvents;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.Mcp.Transport;
using ADOFAILoom.State;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Mcp
{
    internal static class McpServerFactory
    {
        public static McpServer Create(MainThreadDispatcher dispatcher, Action<string> log)
        {
            var sceneSwitcher = new SceneSwitcher(dispatcher);
            var levelOpener = new LevelOpener(dispatcher);
            var visualEventReader = new VisualEventReader(dispatcher);
            var visualEventMutationEngine = new VisualEventMutationEngine(dispatcher);
            var cameraMoveEventActions = new CameraMoveEventActions(visualEventMutationEngine);
            var filterEventActions = new FilterEventActions(visualEventMutationEngine);
            var editorWorkflow = new EditorWorkflowActions(dispatcher);
            var services = new Dictionary<Type, object>
            {
                [typeof(MainThreadDispatcher)] = dispatcher,
                [typeof(SceneSwitcher)] = sceneSwitcher,
                [typeof(LevelOpener)] = levelOpener,
                [typeof(VisualEventReader)] = visualEventReader,
                [typeof(CameraMoveEventActions)] = cameraMoveEventActions,
                [typeof(FilterEventActions)] = filterEventActions,
                [typeof(EditorWorkflowActions)] = editorWorkflow
            };
            McpToolRegistry tools = McpToolRegistry.Discover(
                typeof(McpServerFactory).Assembly,
                services,
                McpProtocol.JsonOptions);
            var router = new McpRequestRouter(tools, log);
            var transport = new StreamableHttpTransport(router, log);
            return new McpServer(transport);
        }
    }
}
