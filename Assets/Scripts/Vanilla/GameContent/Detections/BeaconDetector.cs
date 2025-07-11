using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class BeaconDetector : Detector
    {
        protected override Bounds GetDetectionBounds(Entity self)
        {
            var source = self.GetShootPoint();

            var projectileID = self.GetProjectileID();
            var projectileDef = GetEntityDefinition(self.Level, projectileID);
            var projectileSize = projectileDef.GetSize();

            var minX = 0;
            var maxX = VanillaLevelExt.LEVEL_WIDTH;
            var minZ = 0;
            var maxZ = VanillaLevelExt.LAWN_HEIGHT;
            var sizeY = 800;
            sizeY = 1000;
            var center = self.GetCenter();
            center.x = (minX + maxX) * 0.5f;
            center.y = source.y + projectileSize.y - sizeY * 0.5f;
            center.z = (minZ + maxZ) * 0.5f;
            return new Bounds(center, new Vector3(maxX - minX, sizeY, maxZ - minZ));
        }
        protected override bool ValidateCollider(DetectionParams param, IEntityCollider collider)
        {
            var self = param.entity;
            if (!ValidateTarget(param, collider.Entity))
                return false;
            var shootOffset = self.GetShotOffset();
            shootOffset = self.ModifyShotOffset(shootOffset);
            var source = self.Position + shootOffset;
            var range = self.GetRange();

            var projectileID = self.GetProjectileID();
            var projectileDef = GetEntityDefinition(self.Level, projectileID);
            var projectileSize = projectileDef.GetSize();
            var radius = Mathf.Max(projectileSize.x, projectileSize.y, projectileSize.z) * 0.5f;

            Bounds targetBounds = collider.GetBoundingBox();
            foreach (var direction in Beacon.shootDirections)
            {
                var destination = direction * range + source;
                var capsule = new Capsule(source, destination, radius);
                if (MathTool.CollideBetweenCubeAndCapsule(capsule, targetBounds))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
