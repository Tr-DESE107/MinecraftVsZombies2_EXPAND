using PVZEngine;
using PVZEngine.LevelManagement;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class DispenserDetector : Detector
    {
        public override bool IsInRange(Entity self, Entity target)
        {
            var targetSize = target.GetSize();
            float enemyHeight = targetSize.y;

            var projectileDef = self.Game.GetEntityDefinition(projectileID);
            var projectileSize = projectileDef.GetSize();
            if (TargetInLawn(target) &&
                TargetInFront(self, target) &&
                Detection.IsZCoincide(self.Pos.z, projectileSize.z, target.Pos.z, targetSize.z))
            {
                if (ignoreHighEnemy)
                {
                    if (ignoreLowEnemy)
                    {
                        return Detection.IsYCoincide(self.Pos.y + shootOffset.y, projectileSize.y, target.Pos.y, enemyHeight);
                    }
                    else
                    {
                        return Detection.IsBelowOf(self, target, shootOffset.y);
                    }
                }
                else
                {
                    if (ignoreLowEnemy)
                    {
                        return Detection.IsOverOf(self, target, shootOffset.y);
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
            return range < 0 ? Detection.IsInFrontOf(self, target, shootOffset.x) : Detection.IsInFrontOf(self, target, shootOffset.x, range);
        }
        public NamespaceID projectileID;
        public float range;
        public bool ignoreLowEnemy;
        public bool ignoreHighEnemy;
        public Vector3 shootOffset;
    }
}
