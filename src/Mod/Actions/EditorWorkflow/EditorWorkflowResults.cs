namespace ADOFAILoom.Actions.EditorWorkflow
{
    internal sealed class EditorHistoryResult
    {
        public EditorHistoryResult(
            string operation,
            string revision,
            bool canUndo,
            bool canRedo)
        {
            Operation = operation;
            Revision = revision;
            CanUndo = canUndo;
            CanRedo = canRedo;
        }

        public string Operation { get; }

        public string Revision { get; }

        public bool CanUndo { get; }

        public bool CanRedo { get; }
    }

    internal sealed class LevelSaveResult
    {
        public LevelSaveResult(string levelPath, string revision, string status)
        {
            LevelPath = levelPath;
            Revision = revision;
            Status = status;
        }

        public string LevelPath { get; }

        public string Revision { get; }

        public string Status { get; }
    }
}
