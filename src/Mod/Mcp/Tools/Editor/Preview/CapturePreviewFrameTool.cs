using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Actions.EditorPreview;
using ADOFAILoom.Mcp.Tooling;

namespace ADOFAILoom.Mcp.Tools
{
    internal sealed class CapturePreviewFrameTool
    {
        private readonly EditorPreviewActions actions;

        public CapturePreviewFrameTool(EditorPreviewActions actions)
        {
            this.actions = actions;
        }

        [McpTool(
            "capture_preview_frame",
            Description = "Capture the active editor preview camera as standard MCP PNG image content.",
            ReadOnly = true,
            Destructive = false,
            Idempotent = false,
            OpenWorld = false
        )]
        [McpImageContent(nameof(PreviewFrameCapture.PngData), "image/png")]
        public Task<PreviewFrameCapture> CapturePreviewFrame(
            [Description("PNG width from 320 through 1920 pixels.")] int width = 1280,
            [Description("PNG height from 180 through 1080 pixels.")] int height = 720,
            CancellationToken cancellationToken = default
        )
        {
            return actions.CaptureFrameAsync(width, height, cancellationToken);
        }
    }
}
