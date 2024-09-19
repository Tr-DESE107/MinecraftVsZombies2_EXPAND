using System.Threading.Tasks;
using MVZ2.Level;
using PVZEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVZ2
{
    public class LevelManager : MonoBehaviour
    {
        public LevelController GetLevel()
        {
            return controller;
        }
        public void StartLevel(NamespaceID areaID, NamespaceID stageID)
        {
            controller.InitGame(Main.Game, areaID, stageID);
        }
        public NamespaceID[] GetSeedPacksID()
        {
            var packs = new NamespaceID[6];
            var unlocked = Main.SaveManager.GetUnlockedContraptions();
            for (int i = 0; i < packs.Length; i++)
            {
                if (i < unlocked.Length)
                {
                    packs[i] = unlocked[i];
                }
            }
            return packs;
        }
        public async Task GotoLevelSceneAsync()
        {
            var sceneName = "Level";
            var oldScene = GetScene(sceneName);
            await LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (oldScene.IsValid())
            {
                await UnloadSceneAsync(oldScene);
            }

            var newScene = GetScene(sceneName);
            foreach (var go in newScene.GetRootGameObjects())
            {
                var ctrl = go.GetComponent<LevelController>();
                if (ctrl)
                {
                    controller = ctrl;
                    break;
                }
            }
        }
        public async Task ExitLevelSceneAsync()
        {
            var sceneName = "Level";
            if (!IsSceneLoaded(sceneName))
                return;
            await UnloadSceneAsync(sceneName);
            controller = null;
        }
        private bool IsSceneLoaded(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            return scene.IsValid();
        }
        private Scene GetScene(string name)
        {
            return SceneManager.GetSceneByName(name);
        }
        private Task LoadSceneAsync(string name, LoadSceneMode mode)
        {
            TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>();
            var op = SceneManager.LoadSceneAsync(name, mode);
            op.completed += (op) => tcs.SetResult(op);

            return tcs.Task;
        }
        private Task UnloadSceneAsync(string name)
        {
            TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>();
            var op = SceneManager.UnloadSceneAsync(name);
            op.completed += (op) => tcs.SetResult(op);

            return tcs.Task;
        }
        private Task UnloadSceneAsync(Scene scene)
        {
            TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>();
            var op = SceneManager.UnloadSceneAsync(scene);
            op.completed += (op) => tcs.SetResult(op);

            return tcs.Task;
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private LevelController controller;
    }
}
