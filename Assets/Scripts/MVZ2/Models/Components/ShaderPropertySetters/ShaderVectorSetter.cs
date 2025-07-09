﻿using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    public class ShaderVectorSetter : ShaderPropertySetter<Vector4>
    {
        public override Vector4 GetCurrentValue() => value;
        public override Vector4 GetDefaultValue() => Vector4.zero;
        public override void SetProperty(Vector4 value)
        {
            Element.SetVector(propertyName, value);
        }
        public string propertyName;
        public Vector4 value;
    }
}
