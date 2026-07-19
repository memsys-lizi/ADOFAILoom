namespace ADOFAILoom.Protocol
{
    public static class BridgeProtocol
    {
        public const int Version = 1;
        public const string PipeName = "ADOFAILoom.GameBridge.v1";
        public const string GetGameStateMethod = "get_game_state";
        public const int MaxMessageBytes = 64 * 1024;
        public const int RequestTimeoutMilliseconds = 2000;
    }
}
