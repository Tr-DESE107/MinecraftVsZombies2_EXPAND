using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class TeslaCoilDetector : Detector
    {
        public TeslaCoilDetector(float attackHeight)
        {
            this.attackHeight = attackHeight;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var range = self.GetRange();
            var sizeX = range * 2;
            var sizeY = attackHeight;
            var sizeZ = range * 2;
            var centerX = self.Position.x;
            var centerY = self.Position.y + sizeY * 0.5f;
            var centerZ = self.Position.z;
            return new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
        }
        protected override bool ValidateCollider(DetectionParams param, EntityCollider collider)
        {
            if (!base.ValidateCollider(param, collider))
                return false;
            var self = param.entity;

            var range = self.GetRange();
            var center = new Vector2(self.Position.x, self.Position.z);

            var hitboxCount = collider.GetHitboxCount();
            for (int i = 0; i < hitboxCount; i++)
            {
                var hitbox = collider.GetHitbox(i);
                var hitboxCenter = hitbox.GetBoundsCenter();
                var hitboxSize = hitbox.GetBoundsSize();
                var hitboxCenter2D = new Vector2(hitboxCenter.x, hitboxCenter.z);
                var hitboxSize2D = new Vector2(hitboxSize.x, hitboxSize.z);
                if (MathTool.CollideBetweenRectangleAndCircle(center, range, hitboxCenter2D, hitboxSize2D))
                {
                    return true;
                }
            }
            return false;
        }
        private float attackHeight;
    }
}
