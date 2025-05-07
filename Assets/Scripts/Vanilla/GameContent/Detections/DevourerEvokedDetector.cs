using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class DevourerEvokedDetector : Detector
    {
        public DevourerEvokedDetector()
        {
        }
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var minX = VanillaLevelExt.LEFT_BORDER;
            var maxX = VanillaLevelExt.RIGHT_BORDER;
            var minZ = self.Level.GetGridBottomZ();
            var maxZ = self.Level.GetGridTopZ();
            var sizeY = 800;
            var centerX = (minX + maxX) * 0.5f;
            var centerY = sizeY * 0.5f;
            var centerZ = (minZ + maxZ) * 0.5f;
            return new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(maxX - minX, sizeY, maxZ - minZ));
        }
        public override bool ValidateTarget(DetectionParams self, Entity target)
        {
            if (!TargetInLawn(target))
                return false;
            if (target.GetRelativeY() > 48 + self.entity.GetRelativeY())
                return false;
            return base.ValidateTarget(self, target);
        }
    }
}
