using UnityEngine;

namespace MVZ2.Models
{
    public class ModelComponent : MonoBehaviour
    {
        public virtual void UpdateLogic() { }
        public virtual void UpdateFrame(float deltaTime) { }
        public virtual void OnPropertySet(string name, object value) { }
        public Model Model { get; set; }
    }
}
