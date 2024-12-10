using MVZ2.Models;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Models;
using UnityEngine;

namespace MVZ2.Models
{
    public abstract class ModelInterface : IModelInterface
    {
        public abstract void ChangeModel(NamespaceID modelID);
        public void TriggerAnimation(string name)
        {
            var targetModel = GetModel();
            if (!targetModel)
                return;
            targetModel.TriggerAnimator(name);
        }
        public void SetAnimationBool(string name, bool value)
        {
            var targetModel = GetModel();
            if (!targetModel)
                return;
            targetModel.SetAnimatorBool(name, value);
        }
        public void SetAnimationInt(string name, int value)
        {
            var targetModel = GetModel();
            if (!targetModel)
                return;
            targetModel.SetAnimatorInt(name, value);
        }
        public void SetAnimationFloat(string name, float value)
        {
            var targetModel = GetModel();
            if (!targetModel)
                return;
            targetModel.SetAnimatorFloat(name, value);
        }
        public void SetModelProperty(string name, object value)
        {
            var model = GetModel();
            if (!model)
                return;
            model.SetProperty(name, value);
        }
        public void SetShaderFloat(string name, float value)
        {
            var model = GetModel();
            if (!model)
                return;
            model.SetShaderFloat(name, value);
        }
        public void SetShaderInt(string name, int value)
        {
            var model = GetModel();
            if (!model)
                return;
            model.SetShaderInt(name, value);
        }
        public void SetShaderColor(string name, Color value)
        {
            var model = GetModel();
            if (!model)
                return;
            model.SetShaderColor(name, value);
        }
        protected abstract Model GetModel();
    }
}
