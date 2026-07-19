using System.Text.Json.Serialization;

namespace ADOFAILoom.Actions
{
    internal sealed class SceneSwitchResult
    {
        public SceneSwitchResult(string sceneName, string status)
        {
            SceneName = sceneName;
            Status = status;
        }

        [JsonPropertyName("sceneName")]
        public string SceneName { get; }

        [JsonPropertyName("status")]
        public string Status { get; }
    }
}
