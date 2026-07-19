using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class OpenLevelTool
    {
        private readonly LevelOpener levelOpener;

        public OpenLevelTool(LevelOpener levelOpener)
        {
            this.levelOpener = levelOpener;
        }

        [McpTool(
            "open_level",
            Description = "Open an existing .adofai file in the level editor.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = true
        )]
        public Task<LevelOpenResult> OpenLevel(
            [Description("Absolute path to an existing .adofai level file.")] string levelPath,
            CancellationToken cancellationToken
        )
        {
            return levelOpener.OpenAsync(levelPath, cancellationToken);
        }
    }
}
