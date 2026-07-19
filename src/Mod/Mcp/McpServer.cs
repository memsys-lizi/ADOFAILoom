using System;
using ADOFAILoom.Mcp.Transport;

namespace ADOFAILoom.Mcp
{
    internal sealed class McpServer : IDisposable
    {
        private readonly StreamableHttpTransport transport;

        public McpServer(StreamableHttpTransport transport)
        {
            this.transport = transport;
        }

        public void Start()
        {
            transport.Start();
        }

        public void Dispose()
        {
            transport.Dispose();
        }
    }
}
