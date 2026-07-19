using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Tooling;
using ADOFAILoom.State;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class ListVisualEventsTool
    {
        private readonly VisualEventReader reader;

        public ListVisualEventsTool(VisualEventReader reader)
        {
            this.reader = reader;
        }

        [McpTool(
            "list_visual_events",
            Description = "List visual level and decoration events with strict references for editing.",
            ReadOnly = true,
            Destructive = false,
            Idempotent = true,
            OpenWorld = false
        )]
        public Task<VisualEventPage> ListVisualEvents(
            [McpOptional]
            [Description("Inclusive first floor. Use -1 to include globally placed decorations.")]
                int? floorStart = null,
            [McpOptional]
            [Description(
                "Inclusive last floor. Use -1 to select only globally placed decorations."
            )]
                int? floorEnd = null,
            [McpOptional]
            [Description("Exact, case-sensitive visual LevelEventType names to include.")]
                string[]? eventTypes = null,
            [Description("Zero-based result offset.")] int offset = 0,
            [Description("Page size from 1 through 200.")] int limit = 100,
            CancellationToken cancellationToken = default
        )
        {
            return reader.ListAsync(
                floorStart,
                floorEnd,
                eventTypes,
                offset,
                limit,
                cancellationToken
            );
        }
    }
}
