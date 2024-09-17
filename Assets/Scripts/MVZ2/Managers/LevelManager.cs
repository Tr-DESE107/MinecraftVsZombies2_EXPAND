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
        public async Task GotoLevelScene()
        {
            var sceneName = "Level";
            if (IsSceneLoaded(sceneName))
            {
                await UnloadSceneAsync(sceneName);
            }
            await LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            var scene = SceneManager.GetSceneByName(sceneName);
            foreach (var go in scene.GetRootGameObjects())
            {
                var ctrl = go.GetComponent<LevelController>();
                if (ctrl)
                {
                    controller = ctrl;
                    break;
                }
            }
        }
        private bool IsSceneLoaded(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            return scene.IsValid();
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
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private LevelController controller;
    }
}
