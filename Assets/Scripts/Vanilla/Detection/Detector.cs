using System;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Detections
{
    public abstract class Detector
    {
        public Detector()
        {

        }
        public Entity Detect(Entity self)
        {
            return self.Level.FindFirstEntity(e => Validate(self, e));
        }
        public Entity[] DetectMutiple(Entity self)
        {
            return self.Level.FindEntities(e => Validate(self, e));
        }
        public bool Validate(Entity self, Entity target)
        {
            if (target == null)
                return false;
            if (target.IsDead)
                return false;
            if (!self.IsHostile(target))
                return false;
            if (ignoreBoss && target.Type == EntityTypes.BOSS)
                return false;
            if (!canDetectInvisible && target.IsInvisible())
                return false;
            if (!target.IsVulnerableEntity() && (invulnerableFilter == null || !invulnerableFilter(self, target)))
                return false;
            return IsInRange(self, target);
        }
        public abstract bool IsInRange(Entity self, Entity target);
        protected bool TargetInLawn(Entity target)
        {
            return target.Position.x > VanillaLevelExt.GetAttackBorderX(false) && target.Position.x < VanillaLevelExt.GetAttackBorderX(true);
        }
        public bool canDetectInvisible;
        public bool ignoreBoss;
        public Func<Entity, Entity, bool> invulnerableFilter;
    }
}
