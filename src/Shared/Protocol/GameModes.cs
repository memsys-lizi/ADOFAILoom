namespace ADOFAILoom.Protocol
{
    public static class GameModes
    {
        public const string Menu = "menu";
        public const string LevelSelect = "level_select";
        public const string Editor = "editor";
        public const string Gameplay = "gameplay";
        public const string Unknown = "unknown";
    }

    public static class GameModeResolver
    {
        public static string Resolve(
            string? scene,
            bool isEditor,
            bool isGameplay,
            bool isLevelSelect)
        {
            if (isEditor)
            {
                return GameModes.Editor;
            }

            if (isGameplay)
            {
                return GameModes.Gameplay;
            }

            if (isLevelSelect)
            {
                return GameModes.LevelSelect;
            }

            return string.IsNullOrWhiteSpace(scene) ? GameModes.Unknown : GameModes.Menu;
        }
    }
}
