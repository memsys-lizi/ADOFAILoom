namespace ADOFAILoom.Actions.EditorPreview
{
    internal sealed class EditorPreviewResult
    {
        public EditorPreviewResult(string status, int floor, string revision)
        {
            Status = status;
            Floor = floor;
            Revision = revision;
        }

        public string Status { get; }

        public int Floor { get; }

        public string Revision { get; }
    }
}
