﻿using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    public class ColorOffsetSetter : ShaderPropertySetter<Color>
    {
        public override Color GetCurrentValue() => color;
        public override Color GetDefaultValue() => Color.clear;
        public override void SetProperty(Color value)
        {
            Element.SetColor("_ColorOffset", value);
        }
        public Color color;
    }
}
