using UnityEngine;

namespace MVZ2
{
    public class Shake
    {
        public Shake(float startAmplitude, float endAmplitude, int timeout)
        {
            this.startAmplitude = startAmplitude;
            this.endAmplitude = endAmplitude;
            this.timeout = timeout;
            maxTimeout = timeout;
        }
        public float GetAmplitude()
        {
            return Mathf.Lerp(startAmplitude, endAmplitude, timeout / (float)maxTimeout);
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
        public int timeout;
        public int maxTimeout;
    }
}
