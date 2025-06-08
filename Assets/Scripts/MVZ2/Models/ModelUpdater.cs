using UnityEngine;

namespace MVZ2.Models
{
    public class ModelUpdater : MonoBehaviour
    {
        void Update()
        {
            if (model)
            {
                var deltaTime = Time.deltaTime;
                model.UpdateFrame(deltaTime);
                model.UpdateAnimators(deltaTime);
            }
        }
        private void FixedUpdate()
        {
            if (model)
            {
                model.UpdateFixed();
            }
        }

        public Model model;
    }
}
