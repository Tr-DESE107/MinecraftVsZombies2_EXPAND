﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MVZ2Logic
{
    public abstract class Shake
    {
        public Shake(float startAmplitude, float endAmplitude)
        {
            this.startAmplitude = startAmplitude;
            this.endAmplitude = endAmplitude;
        }
        public float GetAmplitude()
        {
            return Mathf.Lerp(startAmplitude, endAmplitude, GetTimePercentage());
        }
        protected abstract float GetTimePercentage();
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
    }
    [Serializable]
    public class ShakeInt : Shake
    {
        public ShakeInt(float startAmplitude, float endAmplitude, int timeout) : base(startAmplitude, endAmplitude)
        {
            this.timeout = timeout;
            maxTimeout = timeout;
        }
        public void Run()
        {
            timeout--;
        }
        protected override float GetTimePercentage()
        {
            return 1 - timeout / (float)maxTimeout;
        }
        public bool Expired => timeout <= 0;
        public int timeout;
        public int maxTimeout;
    }
    [Serializable]
    public class ShakeFloat : Shake
    {
        public ShakeFloat(float startAmplitude, float endAmplitude, float timeout) : base(startAmplitude, endAmplitude)
        {
            this.timeout = timeout;
            maxTimeout = timeout;
        }
        public void Run(float speed)
        {
            timeout -= speed;
        }
        protected override float GetTimePercentage()
        {
            return 1 - timeout / maxTimeout;
        }
        public bool Expired => timeout <= 0;
        public float timeout;
        public float maxTimeout;
    }
}
