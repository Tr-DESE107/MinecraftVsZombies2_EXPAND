using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class GameUpdater : MonoBehaviour
    {
        private void Update()
        {
            var deltaTime = Time.deltaTime;
            timeModular += deltaTime;

            var fixedInterval = 1 / (float)logicTicksPerSeconds;

            var level = main.LevelManager.GetLevel();
            if (timeModular > fixedInterval)
            {
                int updateTimes = (int)(timeModular / fixedInterval);
                updateTimes = Mathf.Min(updateTimes, maxUpdateTimePerFrame);
                for (int i = 0; i < updateTimes; i++)
                {
                    if (level)
                    {
                        level.UpdateLogic();
                    }
                    main.UpdateManagerFixed();
                }
                timeModular %= fixedInterval;
            }
            var updateDeltaTime = Mathf.Min(fixedInterval * maxUpdateTimePerFrame, deltaTime);
            if (level)
            {
                Physics2D.SyncTransforms();
                level.UpdateFrame(updateDeltaTime);
            }
        }
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private int logicTicksPerSeconds = 30;
        [SerializeField]
        private int maxUpdateTimePerFrame = 1;
        private float timeModular = 0;
    }
}
