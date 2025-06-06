﻿using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class MutantZombieDetector : Detector
    {
        public MutantZombieDetector(float rangeAddition)
        {
            this.rangeAddition = rangeAddition;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var sizeX = self.GetRange() + rangeAddition;
            var sizeY = self.GetMaxAttackHeight();
            var sizeZ = 80;
            var pos = self.Position;
            var centerX = pos.x + self.GetFacingX() * sizeX * 0.5f;
            var centerY = pos.y + sizeY * 0.5f;
            var centerZ = pos.z;
            return new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
        }
        private float rangeAddition;
    }
}
