using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class DispenserDetector : Detector
    {
        public override bool IsInRange(Entity self, Entity target)
        {
            var targetSize = target.GetSize();
            float enemyHeight = targetSize.y;

            var projectileID = self.GetProjectileID();
            var shootOffset = self.GetShotOffset();
            var projectileDef = self.Level.ContentProvider.GetEntityDefinition(projectileID);
            var projectileSize = projectileDef.GetSize();
            if (TargetInLawn(target) &&
                TargetInFront(self, target) &&
                Detection.IsZCoincide(self.Pos.z, projectileSize.z, target.Pos.z, targetSize.z))
            {
                if (ignoreHighEnemy)
                {
                    if (ignoreLowEnemy)
                    {
                        return Detection.IsYCoincide(target.Pos.y, enemyHeight, self.Pos.y + shootOffset.y, projectileSize.y);
                    }
                    else
                    {
                        return target.CoincidesYDown(self.Pos.y + shootOffset.y + projectileSize.y);
                    }
                }
                else
                {
                    if (ignoreLowEnemy)
                    {
                        return target.CoincidesYUp(self.Pos.y + shootOffset.y);
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
