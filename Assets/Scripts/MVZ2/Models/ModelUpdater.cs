using UnityEngine;

namespace MVZ2.Models
{
    public class ModelUpdater : MonoBehaviour
    {
        void Update()
        {
            if (model)
            {
                model.UpdateFrame(Time.deltaTime);
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
