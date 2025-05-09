using UnityEngine;

namespace MVZ2.Models
{
    public class FireballTrailSetter : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            foreach (var trail in trails)
            {
                trail.Init();
            }
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            foreach (var trail in trails)
            {
                trail.UpdateLogic();
            }
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            foreach (var trail in trails)
            {
                trail.UpdateFrame();
            }
        }
        [SerializeField]
        private TrailController[] trails;
    }
}
