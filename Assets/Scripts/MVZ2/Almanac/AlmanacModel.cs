using MVZ2.Models;
using UnityEngine;

namespace MVZ2.Almanacs
{
    public class AlmanacModel : MonoBehaviour
    {
        public void ChangeModel(Model prefab, Camera camera)
        {
            if (model)
            {
                Destroy(model.gameObject);
                model = null;
                updater.model = null;
            }
            if (prefab)
            {
                model = Model.Create(prefab, rootTransform, camera);
                updater.model = model;
            }
        }
        [SerializeField]
        private Transform rootTransform;
        [SerializeField]
        private ModelUpdater updater;
        [SerializeField]
        private Model model;
    }
}
