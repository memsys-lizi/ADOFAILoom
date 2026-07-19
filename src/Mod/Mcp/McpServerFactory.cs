using System;
using System.Collections.Generic;
using ADOFAILoom.Actions;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.Mcp.Transport;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Mcp
{
    internal static class McpServerFactory
    {
        public static McpServer Create(MainThreadDispatcher dispatcher, Action<string> log)
        {
            var sceneSwitcher = new SceneSwitcher(dispatcher);
            var levelOpener = new LevelOpener(dispatcher);
            var services = new Dictionary<Type, object>
            {
                [typeof(MainThreadDispatcher)] = dispatcher,
                [typeof(SceneSwitcher)] = sceneSwitcher,
                [typeof(LevelOpener)] = levelOpener
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
