﻿using UnityEngine;

namespace MVZ2.Models
{
    public class CrushingWallsModelPlatform : MonoBehaviour
    {
        public void SetLeftPosition(Camera camera, float value, Vector3 shake)
        {
            SetWallPosition(camera, value, shake, leftWall, leftStart, leftEnd);
        }
        public void SetRightPosition(Camera camera, float value, Vector3 shake)
        {
            SetWallPosition(camera, value, shake, rightWall, rightStart, rightEnd);
        }
        public void SetWallPosition(Camera camera, float value, Vector3 shake, Transform wall, Vector3 start, Vector3 end)
        {
            var worldPosition = camera.ViewportToWorldPoint(Vector3.Lerp(start, end, value));
            var localPosition = wall.parent.InverseTransformPoint(worldPosition) + shake;
            localPosition.z = 0;
            wall.localPosition = localPosition;
        }
        [SerializeField]
        private Vector3 leftStart = new Vector3(0f, 0.5f, 0);
        [SerializeField]
        private Vector3 leftEnd = new Vector3(0.5f, 0.5f, 0);
        [SerializeField]
        private Vector3 rightStart = new Vector3(1f, 0.5f, 0);
        [SerializeField]
        private Vector3 rightEnd = new Vector3(0.5f, 0.5f, 0);
        [SerializeField]
        private Transform leftWall;
        [SerializeField]
        private Transform rightWall;
    }
}
