using System;
using ADOFAILoom.Mcp.Tools;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Mcp
{
    internal static class McpServerFactory
    {
        public static McpHttpServer Create(
            MainThreadDispatcher dispatcher,
            Action<string> log)
        {
            var registry = new McpToolRegistry(new IMcpTool[]
            {
                new GetGameStateTool(dispatcher)
            });

            return new McpHttpServer(new McpRequestHandler(registry), log);
        }
    }
}
