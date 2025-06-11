﻿using Tools;
using UnityEngine;

namespace MVZ2.Models
{
    [RequireComponent(typeof(LineRenderer))]
    public class LightningGenerator : MonoBehaviour
    {
        public void GenerateLightning(int pointCount, Vector3 sourcePosition, Vector3 targetPosition, RandomGenerator rng)
        {
            if (pointCount < 2)
            {
                Debug.LogError("The position count of line renderer is less than 2.");
                return;
            }

            LineRenderer.positionCount = pointCount;

            Vector2 offset = Vector2.zero;

            float dis = (targetPosition - sourcePosition).magnitude;
            Vector3 dir = (targetPosition - sourcePosition).normalized;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, dir);

            Vector3 currentPos;
            for (int i = 0; i < pointCount; i++)
            {
                if (i == pointCount - 1)
                {
                    currentPos = targetPosition;
                }
                else if (i > 0)
                {
                    float percent = (float)i / (pointCount - 1);
                    Vector2 value = Vector2.zero;
                    value.x = (float)rng.Next(0, amplitude) - amplitude / 2;
                    value.y = (float)rng.Next(0, amplitude) - amplitude / 2;
                    if (i > pointCount / 2)
                    {
                        float fraction = Mathf.Pow(percent * 2 - 1, 8);
                        value -= offset * fraction;
                    }

                    offset += value;
                    currentPos = sourcePosition + rotation * new Vector3(percent * dis, offset.x, offset.y);
                }
                else
                {
                    currentPos = sourcePosition;
                }
                LineRenderer.SetPosition(i, currentPos);
            }
        }
        public LineRenderer LineRenderer
        {
            get
            {
                if (!lineRenderer)
                {
                    lineRenderer = GetComponent<LineRenderer>();
                }
                return lineRenderer;
            }
        }
        [SerializeField]
        private LineRenderer lineRenderer;
        public float amplitude = 0.2f;
    }
}