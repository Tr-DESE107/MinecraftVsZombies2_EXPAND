using System.Collections.Generic;
using UnityEngine;

namespace MVZ2.Managers
{
    public class ShakeManager : MonoBehaviour
    {
        public void AddShake(float shakeAmp, float endAmp, float shakeTime)
        {
            shakes.Add(new Shake(shakeAmp, endAmp, shakeTime));
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
                shake.timeout -= Time.deltaTime;
            }
            shakes.RemoveAll(s => s.timeout <= 0);
        }
        private MainManager Main => MainManager.Instance;
        private OptionsManager OptionsManager => Main.OptionsManager;
        private List<Shake> shakes = new List<Shake>();
    }
    public class Shake
    {
        public Shake(float startAmplitude, float endAmplitude, float timeout)
        {
            this.startAmplitude = startAmplitude;
            this.endAmplitude = endAmplitude;
            this.timeout = timeout;
            maxTimeout = timeout;
        }
        public float GetAmplitude()
        {
            return Mathf.Lerp(startAmplitude, endAmplitude, 1 - timeout / maxTimeout);
        }
        public Vector2 GetShake2D()
        {
            var radius = Random.Range(0, GetAmplitude());
            var angle = Random.Range(0, 360);
            var rad = Mathf.Deg2Rad * angle;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        }
        public Vector3 GetShake3D()
        {
            var radius = Random.Range(0, GetAmplitude());
            var angleX = Random.Range(0, 360);
            var angleY = Random.Range(0, 360);
            var angleZ = Random.Range(0, 360);
            var quaternion = Quaternion.Euler(angleX, angleY, angleZ);
            return quaternion * Vector3.right * radius;
        }
        public float startAmplitude;
        public float endAmplitude;
        public float timeout;
        public float maxTimeout;
    }
}
