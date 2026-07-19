using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADOFAILoom.Mcp.Protocol;
using ADOFAILoom.Threading;
using UnityEngine;

namespace ADOFAILoom.Actions
{
    internal sealed class SceneSwitcher
    {
        private const string Started = "started";
        private const string AwaitingConfirmation = "awaiting_confirmation";

        private readonly MainThreadDispatcher dispatcher;

        public SceneSwitcher(MainThreadDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public Task<SceneSwitchResult> SwitchAsync(
            string sceneName,
            CancellationToken cancellationToken
        )
        {
            ValidateSceneName(sceneName);
            return dispatcher.InvokeAsync(
                () => SwitchOnMainThread(sceneName),
                McpProtocol.MainThreadTimeout,
                cancellationToken
            );
        }

        private static SceneSwitchResult SwitchOnMainThread(string sceneName)
        {
            bool isBuiltInScene = Application.CanStreamedLevelBeLoaded(sceneName);
            bool isInstalledDlcScene = DLCManager.DLCManagers.Any(manager =>
                manager.installed && manager.IsDLCSceneOrLevel(sceneName)
            );
            if (!isBuiltInScene && !isInstalledDlcScene)
            {
                throw new ArgumentException(
                    $"Scene '{sceneName}' is not available in this game installation.",
                    nameof(sceneName)
                );
            }

            scrLoader loader = ADOBase.loader;
            if (loader == null)
            {
                throw new InvalidOperationException("The game scene loader is not available.");
            }

            bool started = false;
            void StartTransition()
            {
                loader.LoadSceneWithTransition(WipeDirection.StartsFromRight, sceneName);
                started = true;
            }

            scnEditor editor = scnEditor.instance;
            if (editor == null)
            {
                StartTransition();
            }
            else
            {
                editor.CheckUnsavedChanges(StartTransition);
            }

            return new SceneSwitchResult(sceneName, started ? Started : AwaitingConfirmation);
        }

        private static void ValidateSceneName(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                throw new ArgumentException("Scene name cannot be empty.", nameof(sceneName));
            }

            if (!string.Equals(sceneName, sceneName.Trim(), StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "Scene name cannot contain leading or trailing whitespace.",
                    nameof(sceneName)
                );
            }

            if (sceneName.IndexOf('/') >= 0 || sceneName.IndexOf('\\') >= 0)
            {
                throw new ArgumentException(
                    "Pass a scene name, not a scene asset path.",
                    nameof(sceneName)
                );
            }
        }
    }
}
