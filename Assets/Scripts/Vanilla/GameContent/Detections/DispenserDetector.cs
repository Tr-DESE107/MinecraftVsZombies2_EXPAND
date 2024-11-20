using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Detections
{
    public class DispenserDetector : Detector
    {
        public override bool IsInRange(Entity self, Entity target)
        {
            var targetSize = target.GetScaledSize();
            float enemyHeight = targetSize.y;

            var projectileID = self.GetProjectileID();
            var shootOffset = self.GetShotOffset();
            var projectileDef = self.Level.Content.GetEntityDefinition(projectileID);
            var projectileSize = projectileDef.GetProperty<Vector3>(EngineEntityProps.SIZE);
            if (TargetInLawn(target) &&
                TargetInFront(self, target) &&
                Detection.IsZCoincide(self.Position.z, projectileSize.z, target.Position.z, targetSize.z))
            {
                if (ignoreHighEnemy)
                {
                    if (ignoreLowEnemy)
                    {
                        return Detection.IsYCoincide(target.Position.y, enemyHeight, self.Position.y + shootOffset.y, projectileSize.y);
                    }
                    else
                    {
                        return target.CoincidesYDown(self.Position.y + shootOffset.y + projectileSize.y);
                    }
                }
                else
                {
                    if (ignoreLowEnemy)
                    {
                        return target.CoincidesYUp(self.Position.y + shootOffset.y);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TargetInFront(Entity self, Entity target)
        {
            var shootOffset = self.GetShotOffset();
            var range = self.GetRange();
            return range < 0 ? Detection.IsInFrontOf(self, target, shootOffset.x) : Detection.IsInFrontOf(self, target, shootOffset.x, range);
        }
        public bool ignoreLowEnemy;
        public bool ignoreHighEnemy;
    }
}
