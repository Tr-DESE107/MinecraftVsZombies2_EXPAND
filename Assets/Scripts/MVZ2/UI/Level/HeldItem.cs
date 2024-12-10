using MVZ2.Models;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class HeldItem : MonoBehaviour
    {
        public void UpdateModelFixed()
        {
            if (model)
            {
                model.UpdateFixed();
            }
        }
        public void UpdateModelFrame(float deltaTime)
        {
            if (model)
            {
                model.UpdateFrame(deltaTime);
            }
        }
        public void SetModelSimulationSpeed(float speed)
        {
            if (model)
            {
                model.SetSimulationSpeed(speed);
            }
        }
        public void SetModel(Model prefab)
        {
            if (model)
            {
                Destroy(model.gameObject);
                model = null;
            }
            if (prefab)
            {
                model = Instantiate(prefab.gameObject, modelRoot).GetComponent<Model>();
            }
        }
        [SerializeField]
        private Transform modelRoot;
        private Model model;
    }
}
