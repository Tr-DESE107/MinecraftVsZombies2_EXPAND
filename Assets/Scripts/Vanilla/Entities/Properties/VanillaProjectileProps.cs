using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion]
    public static class VanillaProjectileProps
    {
        public static readonly PropertyMeta ROLLS = new PropertyMeta("rolls");
        public static bool Rolls(this Entity entity) => entity.GetProperty<bool>(ROLLS);
        public static readonly PropertyMeta KILL_ON_GROUND = new PropertyMeta("killOnGround");
        public static bool KillOnGround(this Entity entity) => entity.GetProperty<bool>(KILL_ON_GROUND); 
        public static readonly PropertyMeta PIERCING = new PropertyMeta("piercing");
        public static readonly PropertyMeta POINT_TO_DIRECTION = new PropertyMeta("pointToDirection");
        public static readonly PropertyMeta DAMAGE_EFFECTS = new PropertyMeta("damageEffects");
        public static readonly PropertyMeta NO_DESTROY_OUTSIDE_LAWN = new PropertyMeta("noDestroyOutsideLawn");
        public static readonly PropertyMeta NO_HIT_ENTITIES = new PropertyMeta("noHitEntities");
        public static readonly PropertyMeta IGNORED_COLLIDERS = new PropertyMeta("ignoredColliders");

        public static bool PointsTowardDirection(this Entity entity)
        {
            return entity.GetProperty<bool>(POINT_TO_DIRECTION);
        }
        public static bool WillDestroyOutsideLawn(this Entity projectile)
        {
            return !projectile.GetProperty<bool>(NO_DESTROY_OUTSIDE_LAWN);
        }
        public static bool IsPiercing(this Entity projectile)
        {
            return projectile.GetProperty<bool>(PIERCING);
        }
        public static void SetPiercing(this Entity projectile, bool value)
        {
            projectile.SetProperty(PIERCING, value);
        }
        public static NamespaceID[] GetDamageEffects(this Entity projectile)
        {
            return projectile.GetProperty<NamespaceID[]>(DAMAGE_EFFECTS);
        }
        public static void IgnoreProjectileCollider(this Entity entity, IEntityCollider other)
        {
            var colliders = entity.GetBehaviourField<List<EntityColliderReference>>(IGNORED_COLLIDERS);
            if (colliders == null)
            {
                colliders = new List<EntityColliderReference>();
                entity.SetBehaviourField(IGNORED_COLLIDERS, colliders);
            }
            var reference = other.ToReference();
            if (colliders.Contains(reference))
                return;
            colliders.Add(reference);
        }
        public static void DismissProjectileCollider(this Entity entity, IEntityCollider other)
        {
            var colliders = entity.GetBehaviourField<List<EntityColliderReference>>(IGNORED_COLLIDERS);
            if (colliders == null)
            {
                return;
            }
            var reference = other.ToReference();
            colliders.Remove(reference);
        }
        public static bool IsProjectileColliderIgnored(this Entity entity, IEntityCollider other)
        {
            var colliders = entity.GetBehaviourField<List<EntityColliderReference>>(IGNORED_COLLIDERS);
            if (colliders == null)
            {
                return false;
            }
            var reference = other.ToReference();
            return colliders.Contains(reference);
        }
    }
}
