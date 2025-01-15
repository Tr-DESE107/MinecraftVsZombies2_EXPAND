using System;
using System.Collections.Generic;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Detections
{
    public abstract class Detector
    {
        public Detector()
        {

        }
        public Entity Detect(Entity self)
        {
            return self.Level.FindFirstEntity(predicate);

            bool predicate(Entity e)
            {
                return Validate(self, e);
            }
        }
        public Entity DetectOrderBy<TKey>(Entity self, Func<Entity, TKey> keySelector)
        {
            return self.Level.FindFirstEntityWithTheLeast(predicate, keySelector);

            bool predicate(Entity e)
            {
                return Validate(self, e);
            }
        }
        public Entity DetectOrderByDescending<TKey>(Entity self, Func<Entity, TKey> keySelector)
        {
            return self.Level.FindFirstEntityWithTheMost(predicate, keySelector);

            bool predicate(Entity e)
            {
                return Validate(self, e);
            }
        }
        public Entity[] DetectMutiple(Entity self)
        {
            return self.Level.FindEntities(predicate);

            bool predicate(Entity e)
            {
                return Validate(self, e);
            }
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
        protected EntityDefinition GetEntityDefinition(LevelEngine level, NamespaceID entityID)
        {
            if (!definitionCaches.TryGetValue(entityID, out var cache))
            {
                cache = level.Content.GetEntityDefinition(entityID);
                definitionCaches.Add(entityID, cache);
            }
            return cache;
        }
        public bool canDetectInvisible;
        public bool ignoreBoss;
        public Func<Entity, Entity, bool> invulnerableFilter;
        private Dictionary<NamespaceID, EntityDefinition> definitionCaches = new Dictionary<NamespaceID, EntityDefinition>();
    }
}
