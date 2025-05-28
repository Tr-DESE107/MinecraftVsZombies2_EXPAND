﻿using System.Collections.Generic;
using MVZ2.Managers;
using MVZ2.Options;
using MVZ2Logic;
using UnityEngine;

namespace MVZ2.Cameras
{
    public class ShakeManager : MonoBehaviour
    {
        public void AddShake(float shakeAmp, float endAmp, float shakeTime)
        {
            shakes.Add(new ShakeFloat(shakeAmp, endAmp, shakeTime));
#if UNITY_ANDROID || UNITY_IOS
            if (OptionsManager.IsVibration())
            {
                Handheld.Vibrate();
            }
#endif
        }
        public Vector2 GetShake2D()
        {
            Vector2 shake2D = Vector2.zero;
            foreach (var shake in shakes)
            {
                shake2D += shake.GetShake2D();
            }
            var amount = OptionsManager.GetShakeAmount();
            return shake2D * amount;
        }
        public Vector3 GetShake3D()
        {
            Vector3 shake3D = Vector3.zero;
            foreach (var shake in shakes)
            {
                shake3D += shake.GetShake3D();
            }
            var amount = OptionsManager.GetShakeAmount();
            return shake3D * amount;
        }
        private void Update()
        {
            foreach (var shake in shakes)
            {
                shake.Run(Time.deltaTime);
            }
            shakes.RemoveAll(s => s.Expired);
        }
        private MainManager Main => MainManager.Instance;
        private OptionsManager OptionsManager => Main.OptionsManager;
        private List<ShakeFloat> shakes = new List<ShakeFloat>();
    }
}
