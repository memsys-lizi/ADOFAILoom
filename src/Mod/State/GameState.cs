using System.Text.Json.Serialization;

namespace ADOFAILoom.State
{
    internal sealed class GameState
    {
        public GameState(string? scene, string mode, bool? paused)
        {
            Scene = scene;
            Mode = mode;
            Paused = paused;
        }

        [JsonPropertyName("schemaVersion")]
        public int SchemaVersion => 2;

        [JsonPropertyName("scene")]
        public string? Scene { get; }

        [JsonPropertyName("mode")]
        public string Mode { get; }

        [JsonPropertyName("paused")]
        public bool? Paused { get; }
    }
}
