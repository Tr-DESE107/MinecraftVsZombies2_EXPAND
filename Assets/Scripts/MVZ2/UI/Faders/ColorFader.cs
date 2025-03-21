﻿using UnityEngine;

namespace MVZ2.UI
{
    public class ColorFader : Fader<Color>
    {
        protected override Color LerpValue(Color start, Color end, float t)
        {
            return Color.Lerp(start, end, t);
        }
    }
}
