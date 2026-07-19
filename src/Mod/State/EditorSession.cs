using System;

namespace ADOFAILoom.State
{
    internal static class EditorSession
    {
        public static scnEditor RequireReadable()
        {
            scnEditor editor = scnEditor.instance;
            if (editor == null)
            {
                throw new InvalidOperationException(
                    "This tool requires the level editor scene.");
            }

            if (!editor.initialized)
            {
                throw new InvalidOperationException(
                    "The level editor has not finished initializing.");
            }

            if (editor.isLoading)
            {
                throw new InvalidOperationException(
                    "The level editor is currently loading a level.");
            }

            return editor;
        }

        public static scnEditor RequireMutable()
        {
            scnEditor editor = RequireReadable();
            if (editor.playMode)
            {
                throw new InvalidOperationException(
                    "Visual events cannot be modified while the editor is previewing the level.");
            }

            if (editor.lockPathEditing)
            {
                throw new InvalidOperationException(
                    "The editor has path editing locked.");
            }

            if (editor.changingState != 0)
            {
                throw new InvalidOperationException(
                    "The editor is currently changing its undo or redo state.");
            }

            return editor;
        }
    }
}
