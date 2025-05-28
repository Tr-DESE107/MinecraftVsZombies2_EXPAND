﻿using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class BlackholeDetector : Detector
    {
        public BlackholeDetector()
        {
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var radius = self.GetRange();
            var sizeX = radius * 2;
            var sizeY = radius * 2;
            var sizeZ = radius * 2;
            var center = self.GetCenter();
            return new Bounds(center, new Vector3(sizeX, sizeY, sizeZ));
        }
        protected override bool ValidateCollider(DetectionParams param, IEntityCollider collider)
        {
            if (!base.ValidateCollider(param, collider))
                return false;
            var self = param.entity;
            var center = self.GetCenter();
            var radius = self.GetRange();
            return collider.CheckSphere(center, radius);
        }
    }
}
