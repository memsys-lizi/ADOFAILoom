namespace ADOFAILoom.State
{
    public sealed class GameState
    {
        public int SchemaVersion { get; set; } = 1;

        public bool Connected { get; set; }

        public string? Scene { get; set; }

        public string? Mode { get; set; }

        public bool? Paused { get; set; }

    }
}
