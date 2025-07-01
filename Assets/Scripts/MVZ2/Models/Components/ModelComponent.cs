using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public abstract class ModelComponent : MonoBehaviour
    {
        public virtual void Init() { }
        public virtual void UpdateLogic() { }
        public virtual void UpdateFrame(float deltaTime) { }
        public virtual void OnPropertySet(string name, object value) { }
        public virtual void OnTrigger(string name) { }
        protected Vector3 Lawn2TransPosition(Vector3 pos)
        {
            return Main.LevelManager.LawnToTrans(pos);
        }
        protected Vector3 Trans2LawnPosition(Vector3 pos)
        {
            return Main.LevelManager.TransToLawn(pos);
        }
        protected Vector3 Lawn2TransScale(Vector3 scale)
        {
            return Main.LevelManager.LawnToTransScale * scale;
        }
        protected Vector3 Trans2LawnScale(Vector3 scale)
        {
            return Main.LevelManager.TransToLawnScale * scale;
        }
        public MainManager Main => MainManager.Instance;
        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;
            Model = GetComponentInParent<Model>();
            Init();
#endif
        }
        protected virtual void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;
            UpdateFrame(Time.deltaTime);
#endif
        }
        public Model Model { get; set; }
    }
}
