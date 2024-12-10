using UnityEngine;

namespace PVZEngine.Models
{
    public interface IModelInterface
    {
        void TriggerAnimation(string name);
        void SetAnimationBool(string name, bool value);
        void SetAnimationInt(string name, int value);
        void SetAnimationFloat(string name, float value);
        void ChangeModel(NamespaceID id);
        void SetModelProperty(string name, object value);
        void SetShaderInt(string name, int value);
        void SetShaderFloat(string name, float value);
        void SetShaderColor(string name, Color value);
    }
}
