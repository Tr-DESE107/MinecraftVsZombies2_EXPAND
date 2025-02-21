using UnityEngine;

namespace MVZ2.Models
{
    public class ModelInitializer : MonoBehaviour
    {
        void Awake()
        {
            if (model)
            {
                model.Init(modelCamera);
            }
        }

        public Model model;
        public Camera modelCamera;
    }
}
