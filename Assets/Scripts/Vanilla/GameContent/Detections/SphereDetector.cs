using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class SphereDetector : Detector
    {
        public SphereDetector(float radius)
        {
            this.radius = radius;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var sizeX = radius * 2;
            var sizeY = radius * 2;
            var sizeZ = radius * 2;
            var center = self.GetCenter();
            return new Bounds(center, new Vector3(sizeX, sizeY, sizeZ));
        }
        protected override bool ValidateCollider(DetectionParams param, EntityCollider collider)
        {
            if (!base.ValidateCollider(param, collider))
                return false;
            var self = param.entity;
            var center = self.GetCenter();
            var hitboxCount = collider.GetHitboxCount();
            for (int i = 0; i < hitboxCount; i++)
            {
                var hitbox = collider.GetHitbox(i);
                if (MathTool.CollideBetweenCubeAndSphere(center, radius, hitbox.GetBoundsCenter(), hitbox.GetBoundsSize()))
                {
                    return true;
                }
            }
            return false;
        }
        private float radius;
    }
}
