using System.Text.Json.Serialization;

namespace ADOFAILoom.Actions
{
    internal sealed class LevelOpenResult
    {
        public LevelOpenResult(string levelPath, string status)
        {
            LevelPath = levelPath;
            Status = status;
        }

        [JsonPropertyName("levelPath")]
        public string LevelPath { get; }

        [JsonPropertyName("status")]
        public string Status { get; }
    }
}
