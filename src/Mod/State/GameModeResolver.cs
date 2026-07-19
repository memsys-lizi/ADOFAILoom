using System;
using System.Collections.Generic;

namespace ADOFAILoom.State
{
    internal static class GameModeResolver
    {
        public const string Menu = "menu";
        public const string LevelSelect = "level_select";
        public const string Editor = "editor";
        public const string Gameplay = "gameplay";
        public const string Unknown = "unknown";

        private static readonly HashSet<string> MenuScenes = new HashSet<string>(
            StringComparer.Ordinal)
        {
            "scnSplash",
            "scnIntro",
            "scnLoading",
            "scnCalibration"
        };

        public static string Resolve(
            string? scene,
            bool isEditor,
            bool isGameplay,
            bool isLevelSelect,
            bool isCustomLevelSelect)
        {
            if (isEditor)
            {
                return Editor;
            }

            if (isGameplay)
            {
                return Gameplay;
            }

            if (isLevelSelect || isCustomLevelSelect)
            {
                return LevelSelect;
            }

            if (scene != null && MenuScenes.Contains(scene))
            {
                return Menu;
            }

            return Unknown;
        }
    }
}
