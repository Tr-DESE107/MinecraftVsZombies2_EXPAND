using MVZ2.Managers;
using UnityEngine;

namespace MVZ2
{
    public class GameUpdater : MonoBehaviour
    {
        private void FixedUpdate()
        {
            var level = main.LevelManager.GetLevel();
            if (level)
            {
                level.UpdateLogic();
            }
        }
        private void Update()
        {
            var deltaTime = Time.deltaTime;
            var level = main.LevelManager.GetLevel();
            if (level)
            {
                level.UpdateFrame(deltaTime);
            }
        }
        [SerializeField]
        private MainManager main;
    }
}
