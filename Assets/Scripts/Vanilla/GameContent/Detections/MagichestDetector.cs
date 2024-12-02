using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class MagichestDetector : Detector
    {
        public MagichestDetector(float rangeAddition = 0)
        {
            this.rangeAddition = rangeAddition;
            ignoreBoss = true;
        }
        public override bool IsInRange(Entity self, Entity target)
        {
            if (!TargetInLawn(target))
                return false;
            var targetSize = target.GetScaledSize();
            var rangeOffset = Vector3.zero;
            var rangeSize = new Vector3(self.GetRange() + rangeAddition, 48, 48);
            if (!Detection.IsInFrontOf(self, target, rangeOffset.x, rangeSize.x))
                return false;
            if (!Detection.IsYCoincide(self.Position.y + rangeOffset.y, rangeSize.y, target.Position.y, targetSize.y))
                return false;
            if (!Detection.IsZCoincide(self.Position.z + rangeOffset.z, rangeSize.z, target.Position.z, targetSize.z))
                return false;
            return true;
        }
        private float rangeAddition;
    }
}
