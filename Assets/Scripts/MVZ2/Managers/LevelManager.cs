using System.Collections.Generic;
using System.Threading.Tasks;
using MVZ2.Level;
using PVZEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVZ2
{
    public class LevelManager : MonoBehaviour
    {
        public void StartLevel()
        {
            controller.SetMainManager(main);
            controller.InitGame();
        }
        public async Task GotoLevelScene()
        {
            TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>();
            var op = SceneManager.LoadSceneAsync("Level", LoadSceneMode.Additive);
            op.completed += (op) => tcs.SetResult(op);

            await tcs.Task;

            var scene = SceneManager.GetSceneByName("Level");
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
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        private LevelController controller;
    }
}
