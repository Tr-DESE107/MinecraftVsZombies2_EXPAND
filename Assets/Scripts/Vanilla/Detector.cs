using System.Linq;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class Detector
    {
        public Detector()
        {

        }
        public Entity Detect(Entity self)
        {
            return DetectMutiple(self).FirstOrDefault();
        }
        public Entity[] DetectMutiple(Entity self)
        {
            return self.Game.FindEntities(e => Validate(self, e));
        }
        public bool Validate(Entity self, Entity target)
        {
            if (target.IsDead)
                return false;
            if (!self.IsEnemy(target))
                return false;
            if (ignoreBoss && target.Type == EntityTypes.BOSS)
                return false;
            if (!canDetectInvisible && target.IsInvisible())
                return false;
            return IsInRange(self, target);
        }
        public abstract bool IsInRange(Entity self, Entity target);
        protected bool TargetInLawn(Entity target)
        {
            return target.Pos.x > MVZ2Game.GetAttackBorderX(false) && target.Pos.x < MVZ2Game.GetAttackBorderX(true);
        }
        public bool canDetectInvisible;
        public bool ignoreBoss;
    }
}
