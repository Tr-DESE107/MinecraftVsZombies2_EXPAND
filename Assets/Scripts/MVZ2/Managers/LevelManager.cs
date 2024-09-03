using System.Threading.Tasks;
using MVZ2.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVZ2
{
    public class LevelManager : MonoBehaviour
    {
        public void StartLevel()
        {
            controller.SetMainManager(main);
            controller.InitGame(Main.Game);
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
        public async Task Retry()
        {
            await GotoLevelScene();
            StartLevel();
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
