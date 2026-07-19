using System.Text.Json.Serialization;

namespace ADOFAILoom.Protocol
{
    public sealed class GameState
    {
        [JsonPropertyName("schemaVersion")]
        public int SchemaVersion { get; set; } = 1;

        [JsonPropertyName("connected")]
        public bool Connected { get; set; }

        [JsonPropertyName("scene")]
        public string? Scene { get; set; }

        [JsonPropertyName("mode")]
        public string? Mode { get; set; }

        [JsonPropertyName("paused")]
        public bool? Paused { get; set; }

        public static GameState Disconnected()
        {
            return new GameState
            {
                Connected = false,
                Scene = null,
                Mode = null,
                Paused = null
            };
        }
    }
}
