using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Actions
{
    internal sealed class LevelOpener
    {
        private const string Started = "started";
        private const string AwaitingConfirmation = "awaiting_confirmation";

        private readonly MainThreadDispatcher dispatcher;

        public LevelOpener(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task<LevelOpenResult> OpenAsync(
            string levelPath,
            CancellationToken cancellationToken
        )
        {
            string fullPath = ValidateLevelPath(levelPath);
            return dispatcher.InvokeAsync(
                () => OpenOnMainThread(fullPath),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        private static LevelOpenResult OpenOnMainThread(string levelPath)
        {
            if (!File.Exists(levelPath))
            {
                throw new FileNotFoundException("The level file no longer exists.", levelPath);
            }

            scnEditor editor = scnEditor.instance;
            if (editor == null)
            {
                throw new InvalidOperationException(
                    "open_level requires the scnEditor scene. Call switch_scene first."
                );
            }

            bool started = false;
            void StartOpen()
            {
                editor.OpenLevel(levelPath);
                started = true;
            }

            editor.CheckUnsavedChanges(StartOpen);
            return new LevelOpenResult(levelPath, started ? Started : AwaitingConfirmation);
        }

        private static string ValidateLevelPath(string levelPath)
        {
            if (string.IsNullOrWhiteSpace(levelPath))
            {
                throw new ArgumentException("Level path cannot be empty.", nameof(levelPath));
            }

            if (!Path.IsPathRooted(levelPath))
            {
                throw new ArgumentException(
                    "Level path must be an absolute file path.",
                    nameof(levelPath)
                );
            }

            string fullPath = Path.GetFullPath(levelPath);
            if (
                !string.Equals(
                    Path.GetExtension(fullPath),
                    ".adofai",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new ArgumentException(
                    "Level path must reference an .adofai file.",
                    nameof(levelPath)
                );
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("The level file does not exist.", fullPath);
            }

            return fullPath;
        }
    }
}
