using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.State;
using ADOFAILoom.Threading;

namespace ADOFAILoom.Actions.EditorWorkflow
{
    internal sealed class EditorWorkflowActions
    {
        private readonly MainThreadDispatcher dispatcher;

        public EditorWorkflowActions(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task<EditorHistoryResult> UndoAsync(
            string expectedRevision,
            CancellationToken cancellationToken
        )
        {
            return dispatcher.InvokeAsync(
                () => ChangeHistory(expectedRevision, redo: false),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        public Task<EditorHistoryResult> RedoAsync(
            string expectedRevision,
            CancellationToken cancellationToken
        )
        {
            return dispatcher.InvokeAsync(
                () => ChangeHistory(expectedRevision, redo: true),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        public Task<LevelSaveResult> SaveAsync(
            string expectedRevision,
            CancellationToken cancellationToken
        )
        {
            return dispatcher.InvokeAsync(
                () => Save(expectedRevision),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        private static EditorHistoryResult ChangeHistory(string expectedRevision, bool redo)
        {
            scnEditor editor = EditorSession.RequireMutable();
            EditorStateProvider.RequireRevision(editor, expectedRevision);
            if (redo)
            {
                if (editor.redoStates.Count == 0)
                {
                    throw new InvalidOperationException("The editor redo stack is empty.");
                }

                editor.Redo();
            }
            else
            {
                if (editor.undoStates.Count == 0)
                {
                    throw new InvalidOperationException("The editor undo stack is empty.");
                }

                editor.Undo();
            }

            return new EditorHistoryResult(
                redo ? "redo" : "undo",
                CanonicalJsonHash.ComputeLevelRevision(editor.levelData),
                editor.undoStates.Count > 0,
                editor.redoStates.Count > 0
            );
        }

        private static LevelSaveResult Save(string expectedRevision)
        {
            scnEditor editor = EditorSession.RequireMutable();
            string revision = EditorStateProvider.RequireRevision(editor, expectedRevision);
            string levelPath = ADOBase.levelPath;
            if (string.IsNullOrEmpty(levelPath) || !Path.IsPathRooted(levelPath))
            {
                throw new InvalidOperationException(
                    "The current editor level does not have an absolute file path."
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
                throw new InvalidOperationException(
                    "The current editor path is not an .adofai file."
                );
            }

            editor.SaveLevel();
            if (!File.Exists(fullPath))
            {
                throw new IOException($"The editor did not create the level file '{fullPath}'.");
            }

            string savedJson = File.ReadAllText(fullPath);
            string savedRevision;
            try
            {
                savedRevision = CanonicalJsonHash.ComputeJsonHash(savedJson);
            }
            catch (Exception exception)
                when (exception is System.Text.Json.JsonException
                    || exception is InvalidOperationException
                )
            {
                throw new IOException(
                    $"The saved level file '{fullPath}' is not valid JSON.",
                    exception
                );
            }

            if (!string.Equals(savedRevision, revision, StringComparison.Ordinal))
            {
                throw new IOException(
                    "The saved level file does not match the current editor revision."
                );
            }

            return new LevelSaveResult(fullPath, revision, "saved");
        }
    }
}
