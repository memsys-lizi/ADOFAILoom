using System;
using System.Collections.Generic;
using System.Linq;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class McpToolRegistry
    {
        private readonly List<IMcpTool> tools = new List<IMcpTool>();
        private readonly Dictionary<string, IMcpTool> toolsByName =
            new Dictionary<string, IMcpTool>(StringComparer.Ordinal);

        public McpToolRegistry(IEnumerable<IMcpTool> tools)
        {
            foreach (IMcpTool tool in tools)
            {
                Register(tool);
            }
        }

        public object[] GetDefinitions()
        {
            return tools.Select(tool => tool.Definition).ToArray();
        }

        public bool TryGet(string name, out IMcpTool tool)
        {
            return toolsByName.TryGetValue(name, out tool!);
        }

        private void Register(IMcpTool tool)
        {
            if (string.IsNullOrWhiteSpace(tool.Name))
            {
                throw new ArgumentException("MCP tool names cannot be empty.", nameof(tool));
            }

            if (!toolsByName.TryAdd(tool.Name, tool))
            {
                throw new InvalidOperationException($"Duplicate MCP tool name: {tool.Name}");
            }

            tools.Add(tool);
        }
    }
}
