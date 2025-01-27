using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class SkeletonHorseBlocksJumpDetector : Detector
    {
        protected override Bounds GetDetectionBounds(Entity self)
        {
            return self.GetBounds();
        }
        protected override bool ValidateCollider(DetectionParams param, EntityCollider collider)
        {
            if (!base.ValidateCollider(param, collider))
                return false;
            var bounds = collider.GetBoundingBox();
            if (!Detection.IsInFrontOf(param.entity, bounds.center.x))
                return false;
            var target = collider.Entity;
            if (!target.BlocksJump())
                return false;
            return true;
        }
    }
}
