using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ADOFAILoom.Mcp.Tools
{
    internal interface IMcpTool
    {
        string Name { get; }

        object Definition { get; }

        Task<McpToolCallResult> ExecuteAsync(
            JsonElement? arguments,
            CancellationToken cancellationToken);
    }
}
