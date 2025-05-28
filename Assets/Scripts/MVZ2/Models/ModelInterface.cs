﻿using PVZEngine;
using PVZEngine.Models;
using UnityEngine;

namespace MVZ2.Models
{
    public abstract class ModelInterface : IModelInterface
    {
        public void UpdateModel()
        {
            var targetModel = GetModel();
            if (!targetModel)
                return;
            targetModel.UpdateFrame(0);
        }
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
        public void TriggerModel(string name)
        {
            var model = GetModel();
            if (!model)
                return;
            model.TriggerModel(name);
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
        public IModelInterface CreateChildModel(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            var model = GetModel();
            if (!model)
                return null;
            var child = model.CreateChildModel(anchorName, key, modelID);
            if (child == null)
                return null;
            return child.GetParentModelInterface();
        }
        public bool RemoveChildModel(NamespaceID key)
        {
            var model = GetModel();
            if (!model)
                return false;
            return model.RemoveChildModel(key);
        }
        public IModelInterface GetChildModel(NamespaceID key)
        {
            var model = GetModel();
            if (!model)
                return null;
            var child = model.GetChildModel(key);
            if (!child)
                return null;
            return child.GetParentModelInterface();
        }
        protected abstract Model GetModel();
    }
}
