﻿using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class TheGiantArmDetector : Detector
    {
        public TheGiantArmDetector(bool outer)
        {
            this.outer = outer;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var sizeX = 80 + self.GetScaledSize().x * 0.5f;
            var sizeY = 80;
            var sizeZ = 160;
            var centerX = self.Position.x + sizeX * 0.5f * self.GetFacingX();
            var centerY = self.Position.y + sizeY * 0.5f;
            var centerZ = outer ? self.Position.z - 40 - sizeZ * 0.5f : self.Position.z - 40 + sizeZ * 0.5f;
            return new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
        }
        public bool outer;
    }
}
