using System;
using System.Collections.Generic;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Mcp.Tools;
using ADOFAILoom.Mcp.Transport;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Mcp
{
    internal static class McpServerFactory
    {
        public static McpServer Create(MainThreadDispatcher dispatcher, Action<string> log)
        {
            var services = new Dictionary<Type, object>
            {
                [typeof(MainThreadDispatcher)] = dispatcher
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
