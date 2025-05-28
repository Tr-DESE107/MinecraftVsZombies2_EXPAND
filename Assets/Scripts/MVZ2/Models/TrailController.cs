﻿using UnityEngine;

namespace MVZ2.Models
{
    public class TrailController : MonoBehaviour
    {
        public void Init()
        {
            trailPoints = new Vector3[trail.positionCount];
            worldPositions = new Vector3[trail.positionCount];
            defaultWidthMultiplier = trail.widthMultiplier;
            trail.GetPositions(trailPoints);
            for (int i = 0; i < worldPositions.Length; i++)
            {
                worldPositions[i] = GetCurrentWorldPosition();
                trailPoints[i] = ToTrailPosition(worldPositions[i]);
            }
            trail.SetPositions(trailPoints);
        }
        public void UpdateLogic()
        {
            trail.GetPositions(trailPoints);
            for (int i = worldPositions.Length - 1; i >= 0; i--)
            {
                Vector3 position;
                if (i == 0)
                {
                    position = GetCurrentWorldPosition();
                }
                else
                {
                    position = worldPositions[i - 1] + velocity * i;
                }
                worldPositions[i] = position;
                trailPoints[i] = ToTrailPosition(position);
            }
            trail.SetPositions(trailPoints);
        }
        public void UpdateFrame()
        {
            var worldPos = GetCurrentWorldPosition();
            var position = ToTrailPosition(worldPos);
            trail.SetPosition(0, position);
            if (scaleWithHierarchy)
            {
                var scale = transform.lossyScale;
                trail.widthMultiplier = Mathf.Min(scale.x, scale.y) * defaultWidthMultiplier;
            }
            else
            {
                trail.widthMultiplier = defaultWidthMultiplier;
            }
        }
        private Vector3 ToTrailPosition(Vector3 worldPos)
        {
            if (!trail.useWorldSpace)
            {
                return transform.InverseTransformPoint(worldPos);
            }
            return worldPos;
        }
        private Vector3 GetCurrentWorldPosition()
        {
            return transform.position;
        }
        [SerializeField]
        private LineRenderer trail;
        [SerializeField]
        private Vector3 velocity;
        [SerializeField]
        private bool scaleWithHierarchy;
        private float defaultWidthMultiplier;
        private Vector3[] trailPoints = null;
        private Vector3[] worldPositions = null;
    }
}
