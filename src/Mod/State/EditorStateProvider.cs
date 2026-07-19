using System;
using System.Linq;

namespace ADOFAILoom.State
{
    internal static class EditorStateProvider
    {
        public static EditorState Capture()
        {
            scnEditor editor = EditorSession.RequireReadable();
            string? levelPath = string.IsNullOrEmpty(ADOBase.levelPath) ? null : ADOBase.levelPath;
            int[] selectedFloors = editor.selectedFloors.Select(floor => floor.seqID).ToArray();
            int[] selectedDecorations = editor
                .selectedDecorations.Select(decoration => editor.decorations.IndexOf(decoration))
                .Where(index => index >= 0)
                .ToArray();
            int? currentFloor =
                scrController.instance == null ? (int?)null : scrController.instance.currentSeqID;

            return new EditorState(
                levelPath,
                editor.floors.Count,
                selectedFloors,
                selectedDecorations,
                currentFloor,
                editor.playMode,
                editor.undoStates.Count > 0,
                editor.redoStates.Count > 0,
                CanonicalJsonHash.ComputeLevelRevision(editor.levelData)
            );
        }

        public static string RequireRevision(scnEditor editor, string expectedRevision)
        {
            if (string.IsNullOrEmpty(expectedRevision) || expectedRevision.Length != 64)
            {
                throw new ArgumentException(
                    "Expected revision must be a 64-character lowercase SHA-256 value.",
                    nameof(expectedRevision)
                );
            }

            for (int index = 0; index < expectedRevision.Length; index++)
            {
                char character = expectedRevision[index];
                bool isLowerHex =
                    character >= '0' && character <= '9' || character >= 'a' && character <= 'f';
                if (!isLowerHex)
                {
                    throw new ArgumentException(
                        "Expected revision must be a 64-character lowercase SHA-256 value.",
                        nameof(expectedRevision)
                    );
                }
            }

            string actualRevision = CanonicalJsonHash.ComputeLevelRevision(editor.levelData);
            if (!string.Equals(expectedRevision, actualRevision, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"The editor revision is stale. Expected '{expectedRevision}', "
                        + $"but the current revision is '{actualRevision}'. Read the editor state again."
                );
            }

            return actualRevision;
        }
    }
}
