using ADOFAILoom.Protocol;

namespace ADOFAILoom.State
{
    internal static class GameStateSnapshotProvider
    {
        public static GameState Capture()
        {
            string scene = ADOBase.sceneName;
            bool isEditor = scnEditor.instance != null || ADOBase.isLevelEditor;
            bool isGameplay = scnGame.instance != null || ADOBase.isScnGame;
            bool isLevelSelect = ADOBase.isLevelSelect;
            scrController controller = scrController.instance;

            return new GameState
            {
                Connected = true,
                Scene = scene,
                Mode = GameModeResolver.Resolve(scene, isEditor, isGameplay, isLevelSelect),
                Paused = controller == null ? (bool?)null : controller.paused
            };
        }
    }
}
