namespace ADOFAILoom.State
{
    internal static class GameStateSnapshotProvider
    {
        public static GameState Capture()
        {
            string? scene = ADOBase.sceneName;
            bool isEditor = scnEditor.instance != null || ADOBase.isLevelEditor;
            bool isGameplay = scnGame.instance != null || ADOBase.isScnGame;
            bool isLevelSelect = ADOBase.isLevelSelect;
            bool isCustomLevelSelect = ADOBase.isCLS;
            scrController controller = scrController.instance;

            string mode = GameModeResolver.Resolve(
                scene,
                isEditor,
                isGameplay,
                isLevelSelect,
                isCustomLevelSelect
            );

            return new GameState(scene, mode, controller == null ? (bool?)null : controller.paused);
        }
    }
}
