﻿using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class LawnDetector : Detector
    {
        public LawnDetector()
        {
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var minX = 0;
            var maxX = VanillaLevelExt.LEVEL_WIDTH;
            var minZ = 0;
            var maxZ = VanillaLevelExt.LAWN_HEIGHT;
            var sizeY = 800;
            var center = self.GetCenter();
            center.x = (minX + maxX) * 0.5f;
            center.y = sizeY * 0.5f;
            center.z = (minZ + maxZ) * 0.5f;
            return new Bounds(center, new Vector3(maxX - minX, sizeY, maxZ - minZ));
        }
    }
}
