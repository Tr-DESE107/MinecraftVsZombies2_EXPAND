using MVZ2.Vanilla.Detections;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class FireBreathDetector : Detector
    {
        public override bool IsInRange(Entity self, Entity target)
        {
            var fireBreathDef = GetEntityDefinition(self.Level, fireBreathID);
            if (fireBreathDef == null)
                return false;
            var size = fireBreathDef.GetSize();
            var boundsOffset = fireBreathDef.GetBoundsOffset();
            var positionOffset = offset;
            positionOffset += boundsOffset;
            if (self.IsFacingLeft())
            {
                positionOffset.x *= -1;
            }
            var selfBounds = new Bounds(self.Position + positionOffset, size);
            var targetBounds = target.GetBounds();

            return TargetInLawn(target) && selfBounds.Intersects(targetBounds);
        }
        public NamespaceID fireBreathID;
        public Vector3 offset;
        public static readonly Vector3 defaultPivot = new Vector3(0.5f, 0f, 0.5f);
    }
}
