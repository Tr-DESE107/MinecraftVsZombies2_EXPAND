using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Detections;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class GemMergeDetector : Detector
    {
        public GemMergeDetector()
        {
            mask = EntityCollisionHelper.MASK_PICKUP;
            factionTarget = FactionTarget.Any;
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var size = MergePickup.GetMergeRange(self);
            var sizeX = size;
            var sizeY = size;
            var sizeZ = size;
            var centerX = self.Position.x;
            var centerY = self.Position.y + sizeY * 0.5f;
            var centerZ = self.Position.z;
            return new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
        }
        protected override bool ValidateCollider(DetectionParams param, IEntityCollider collider)
        {
            var gemID = param.entity.GetDefinitionID();
            return collider.Entity.IsEntityOf(gemID);
        }
    }
}
