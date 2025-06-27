﻿using MVZ2.Vanilla.Detections;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class HellfireIgniteDetector : Detector
    {
        public HellfireIgniteDetector(float extraHeight)
        {
            this.extraHeight = extraHeight;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var bounds = self.GetBounds();
            bounds.max += Vector3.up * extraHeight;
            return bounds;
        }
        private float extraHeight;
    }
}
