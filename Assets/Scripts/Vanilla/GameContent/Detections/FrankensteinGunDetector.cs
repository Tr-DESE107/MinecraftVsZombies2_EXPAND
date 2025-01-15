using MVZ2.Vanilla.Detections;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class FrankensteinGunDetector : Detector
    {
        public FrankensteinGunDetector(NamespaceID projectileID)
        {
            this.projectileID = projectileID;
        }
        public override bool IsInRange(Entity self, Entity target)
        {
            if (!Detection.IsInFrontOf(self, target))
                return false;
            if (!TargetInLawn(target))
                return false;
            var projectileDef = GetEntityDefinition(self.Level, projectileID);
            var projectileSize = projectileDef.GetProperty<Vector3>(EngineEntityProps.SIZE);
            var targetSize = target.GetScaledSize();
            return Detection.IsZCoincide(self.Position.z, projectileSize.z, target.Position.z, targetSize.z);
        }
        private NamespaceID projectileID;
    }
}
