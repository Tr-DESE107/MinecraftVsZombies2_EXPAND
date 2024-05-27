using System;
using UnityEngine;

namespace PVZEngine
{
    public partial class Level
    {
        public void ShakeScreen(float startAmplitude, float endAmplitude, int time)
        {
            OnShakeScreen?.Invoke(startAmplitude, endAmplitude, time);
        }
        public event Action<float, float, int> OnShakeScreen;
    }
}