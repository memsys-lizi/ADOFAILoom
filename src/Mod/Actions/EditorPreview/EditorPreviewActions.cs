using System;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.State;
using ADOFAILoom.Threading;
using UnityEngine;

namespace ADOFAILoom.Actions.EditorPreview
{
    internal sealed class EditorPreviewActions
    {
        private readonly MainThreadDispatcher dispatcher;

        public EditorPreviewActions(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task<EditorPreviewResult> StartAsync(
            string expectedRevision,
            int floor,
            CancellationToken cancellationToken
        )
        {
            return dispatcher.InvokeAsync(
                () => Start(expectedRevision, floor),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        public Task<EditorPreviewResult> StopAsync(CancellationToken cancellationToken)
        {
            return dispatcher.InvokeAsync(Stop, McpProtocol.MainThreadTimeout, cancellationToken);
        }

        public Task<PreviewFrameCapture> CaptureFrameAsync(
            int width,
            int height,
            CancellationToken cancellationToken
        )
        {
            return dispatcher.InvokeAsync(
                () => CaptureFrame(width, height),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        private static EditorPreviewResult Start(string expectedRevision, int floor)
        {
            scnEditor editor = EditorSession.RequireReadable();
            if (editor.playMode)
            {
                throw new InvalidOperationException("The editor is already previewing the level.");
            }

            if (editor.changingState != 0)
            {
                throw new InvalidOperationException(
                    "The editor is currently changing its undo or redo state."
                );
            }

            string revision = EditorStateProvider.RequireRevision(editor, expectedRevision);
            if (editor.floors.Count < 2)
            {
                throw new InvalidOperationException(
                    "A level needs at least two floors before preview can start."
                );
            }

            if (floor < 0 || floor >= editor.floors.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(floor),
                    floor,
                    $"Preview floor must be from 0 through {editor.floors.Count - 1}."
                );
            }

            editor.SelectFloor(editor.floors[floor]);
            editor.Play();
            if (!editor.playMode)
            {
                throw new InvalidOperationException("The editor did not enter preview mode.");
            }

            return new EditorPreviewResult("started", floor, revision);
        }

        private static EditorPreviewResult Stop()
        {
            scnEditor editor = EditorSession.RequireReadable();
            if (!editor.playMode)
            {
                throw new InvalidOperationException("The editor is not previewing the level.");
            }

            scrController controller =
                scrController.instance
                ?? throw new InvalidOperationException("The preview controller is unavailable.");
            int floor = controller.currentSeqID;
            editor.SwitchToEditMode();
            if (editor.playMode)
            {
                throw new InvalidOperationException("The editor did not leave preview mode.");
            }

            return new EditorPreviewResult(
                "stopped",
                floor,
                CanonicalJsonHash.ComputeLevelRevision(editor.levelData)
            );
        }

        private static PreviewFrameCapture CaptureFrame(int width, int height)
        {
            scnEditor editor = EditorSession.RequireReadable();
            if (!editor.playMode)
            {
                throw new InvalidOperationException(
                    "A preview frame can only be captured while the editor is previewing the level."
                );
            }

            if (width < 320 || width > 1920)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(width),
                    width,
                    "Capture width must be from 320 through 1920 pixels."
                );
            }

            if (height < 180 || height > 1080)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(height),
                    height,
                    "Capture height must be from 180 through 1080 pixels."
                );
            }

            if ((long)width * height > 2073600)
            {
                throw new ArgumentException("Capture resolution cannot exceed 2,073,600 pixels.");
            }

            RenderTexture previousActive = RenderTexture.active;
            RenderTexture target = RenderTexture.GetTemporary(
                width,
                height,
                24,
                RenderTextureFormat.ARGB32
            );
            Texture2D? texture = null;
            try
            {
                texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                ScreenCapture.CaptureScreenshotIntoRenderTexture(target);
                RenderTexture.active = target;
                texture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                texture.Apply(false, false);
                byte[] png = ImageConversion.EncodeToPNG(texture);
                if (png == null || png.Length == 0)
                {
                    throw new InvalidOperationException("Unity returned an empty PNG frame.");
                }

                scrController controller =
                    scrController.instance
                    ?? throw new InvalidOperationException(
                        "The preview controller is unavailable."
                    );
                return new PreviewFrameCapture(
                    png,
                    width,
                    height,
                    controller.currentSeqID,
                    CanonicalJsonHash.ComputeLevelRevision(editor.levelData)
                );
            }
            finally
            {
                RenderTexture.active = previousActive;
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }

                RenderTexture.ReleaseTemporary(target);
            }
        }
    }
}
