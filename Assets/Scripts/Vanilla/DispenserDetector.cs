using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class DispenserDetector : Detector
    {
        public override bool IsInRange(Entity self, Entity target)
        {
            float enemyHeight = target.Size.y;

            if (TargetInLawn(target) &&
                TargetInFront(self, target) &&
                Detection.IsZCoincide(self.Pos.z, projectileSizeZ, target.Pos.z, target.Size.z))
            {
                if (ignoreHighEnemy)
                {
                    if (ignoreLowEnemy)
                    {
                        return Detection.IsYCoincide(self.Pos.y + shootOffset.y, projectileSizeY, target.Pos.y, enemyHeight);
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
        public float projectileSizeY;
        public float projectileSizeZ;
        public float range;
        public bool ignoreLowEnemy;
        public bool ignoreHighEnemy;
        public Vector3 shootOffset;
    }
}
