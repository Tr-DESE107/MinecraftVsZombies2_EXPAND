using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class ModelComponent : MonoBehaviour
    {
        public virtual void Init() { }
        public virtual void UpdateLogic() { }
        public virtual void UpdateFrame(float deltaTime) { }
        public virtual void OnPropertySet(string name, object value) { }
        protected Vector3 Lawn2TransPosition(Vector3 pos)
        {
            return MainManager.Instance.LevelManager.LawnToTrans(pos);
        }
        protected Vector3 Trans2LawnPosition(Vector3 pos)
        {
            return MainManager.Instance.LevelManager.TransToLawn(pos);
        }
        protected Vector3 Lawn2TransScale(Vector3 scale)
        {
            return MainManager.Instance.LevelManager.LawnToTransScale * scale;
        }
        protected Vector3 Trans2LawnScale(Vector3 scale)
        {
            return MainManager.Instance.LevelManager.TransToLawnScale * scale;
        }
        public Model Model { get; set; }
    }
}
