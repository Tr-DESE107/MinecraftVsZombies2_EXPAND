using System;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class FireworkDispenserDetector : Detector
    {
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var center = GetExplosionCenter(self);
            var range = self.GetRange();
            var size = Vector3.one * (range * 2);
            return new Bounds(center, size);
        }
        protected override bool ValidateCollider(DetectionParams param, IEntityCollider collider)
        {
            if (!ValidateTarget(param, collider.Entity))
                return false;
            var center = GetExplosionCenter(param.entity);
            var range = param.entity.GetRange();
            if (!collider.CheckSphere(center, range))
                return false;
            if (colliderFilter != null && !colliderFilter(param, collider))
                return false;
            return true;
        }
        private Vector3 GetExplosionCenter(Entity self)
        {
            var bulletLifetime = 10;
            var explodePointOffset = self.GetShotOffset() + self.GetShotVelocity() * bulletLifetime;
            return self.Position + explodePointOffset;
        }
        public Func<DetectionParams, IEntityCollider, bool> colliderFilter;
    }
}
