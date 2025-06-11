﻿using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    public class CircleFillSpriteSetter : ShaderPropertySetter<float>
    {
        public override float GetCurrentValue() => fill;
        public override float GetDefaultValue() => 0;
        public override void SetProperty(float value)
        {
            Element.SetFloat("_CircleFill", value);
        }
        [Range(0, 1)]
        public float fill;
    }
}
