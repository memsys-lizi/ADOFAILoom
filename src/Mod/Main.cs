using ADOFAILoom.Bridge;
using ADOFAILoom.Threading;
using UnityModManagerNet;

namespace ADOFAILoom
{
    public static class Main
    {
        private static readonly MainThreadDispatcher Dispatcher = new MainThreadDispatcher();
        private static GameBridgeServer? bridgeServer;

        public static UnityModManager.ModEntry? Mod { get; private set; }

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Mod = modEntry;
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnUnload = OnUnload;
            modEntry.Logger.Log("ADOFAILoom game bridge loaded / 游戏桥接已加载");
            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool enabled)
        {
            if (!enabled)
            {
                StopBridge();
                modEntry.Logger.Log("Game bridge disabled / 游戏桥接已关闭");
                return true;
            }

            try
            {
                bridgeServer ??= new GameBridgeServer(
                    Dispatcher,
                    message => modEntry.Logger.Log(message));
                bridgeServer.Start();
                modEntry.Logger.Log("Game bridge listening on ADOFAILoom.GameBridge.v1");
                return true;
            }
            catch (System.Exception exception)
            {
                StopBridge();
                modEntry.Logger.Error($"Unable to start game bridge: {exception}");
                return false;
            }
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float deltaTime)
        {
            Dispatcher.ProcessPending();
        }

        private static bool OnUnload(UnityModManager.ModEntry modEntry)
        {
            StopBridge();
            Mod = null;
            return true;
        }

        private static void StopBridge()
        {
            bridgeServer?.Dispose();
            bridgeServer = null;
            Dispatcher.CancelPending();
        }
    }
}
