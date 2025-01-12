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
        public void SetModel(Model prefab, Camera camera)
        {
            if (model)
            {
                Destroy(model.gameObject);
                model = null;
            }
            if (prefab)
            {
                model = Model.Create(prefab, modelRoot, camera);
            }
        }
        public Model GetModel()
        {
            return model;
        }
        public void SetTrigger(bool visible, bool trigger)
        {
            triggerObj.SetActive(visible && trigger);
            notTriggerObj.SetActive(visible && !trigger);
        }
        [SerializeField]
        private Transform modelRoot;
        [SerializeField]
        private GameObject triggerObj;
        [SerializeField]
        private GameObject notTriggerObj;
        private Model model;
    }
}
