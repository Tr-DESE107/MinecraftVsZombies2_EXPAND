﻿using UnityEngine;

namespace PVZEngine.Models
{
    public interface IModelInterface
    {
        void UpdateModel();
        void TriggerAnimation(string name);
        void SetAnimationBool(string name, bool value);
        void SetAnimationInt(string name, int value);
        void SetAnimationFloat(string name, float value);
        void SetModelProperty(string name, object value);
        void TriggerModel(string name);
        void SetShaderInt(string name, int value);
        void SetShaderFloat(string name, float value);
        void SetShaderColor(string name, Color value);
        IModelInterface CreateChildModel(string anchor, NamespaceID key, NamespaceID modelID);
        bool RemoveChildModel(NamespaceID key);
        IModelInterface GetChildModel(NamespaceID key);
    }
}
