﻿using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class TheGiantEyeDetector : Detector
    {
        public TheGiantEyeDetector(bool outer)
        {
            this.outer = outer;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var sizeX = 800;
            var sizeY = 800;
            var sizeZ = 800;
            var centerX = self.Position.x + sizeX * 0.5f * self.GetFacingX();
            var centerY = self.GetCenter().y;
            var centerZ = outer ? self.Position.z - sizeZ * 0.5f : self.Position.z + sizeZ * 0.5f;
            return new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
        }
        public bool outer;
    }
}
