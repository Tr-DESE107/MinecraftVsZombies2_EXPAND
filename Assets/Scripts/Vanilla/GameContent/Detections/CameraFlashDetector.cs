using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class CameraFlashDetector : Detector
    {
        public CameraFlashDetector()
        {
            mask = EntityCollisionHelper.MASK_PLANT | EntityCollisionHelper.MASK_PROJECTILE;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            return self.GetBounds();
        }
    }
}
