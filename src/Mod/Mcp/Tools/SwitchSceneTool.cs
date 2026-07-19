using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class SwitchSceneTool
    {
        private readonly SceneSwitcher sceneSwitcher;

        public SwitchSceneTool(SceneSwitcher sceneSwitcher)
        {
            this.sceneSwitcher = sceneSwitcher;
        }

        [McpTool(
            "switch_scene",
            Description = "Switch to an exact Unity scene name, such as scnEditor or scnCLS.",
            ReadOnly = false,
            Destructive = true,
            Idempotent = false,
            OpenWorld = false)]
        public Task<SceneSwitchResult> SwitchScene(
            [Description("Exact Unity scene name, for example scnEditor or scnCLS.")]
            string sceneName,
            CancellationToken cancellationToken)
        {
            return sceneSwitcher.SwitchAsync(sceneName, cancellationToken);
        }
    }
}
