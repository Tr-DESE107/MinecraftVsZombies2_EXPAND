﻿using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class EmperorZombieShieldDetector : Detector
    {
        public EmperorZombieShieldDetector(float radius)
        {
            this.radius = radius;
            factionTarget = FactionTarget.Friendly;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var sizeX = radius * 2;
            var sizeY = radius * 2;
            var sizeZ = radius * 2;
            var center = self.GetCenter();
            return new Bounds(center, new Vector3(sizeX, sizeY, sizeZ));
        }
        public override bool ValidateTarget(DetectionParams self, Entity target)
        {
            if (!base.ValidateTarget(self, target))
                return false;
            if (target.Type != EntityTypes.PLANT && target.Type != EntityTypes.ENEMY)
                return false;
            if (target.HasBuff<DivineShieldBuff>())
                return false;
            if (target.IsNotActiveEnemy())
                return false;
            if (target.IsEntityOf(VanillaEnemyID.emperorZombie))
                return false;
            if ((target.GetCenter() - self.entity.GetCenter()).magnitude > radius)
                return false;
            return true;
        }
        private float radius;
    }
}
