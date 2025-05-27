using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaProjectileProps
    {
        public static readonly PropertyMeta<bool> ROLLS = new PropertyMeta<bool>("rolls");
        public static bool Rolls(this Entity entity) => entity.GetProperty<bool>(ROLLS);
        public static readonly PropertyMeta<bool> KILL_ON_GROUND = new PropertyMeta<bool>("killOnGround");
        public static bool KillOnGround(this Entity entity) => entity.GetProperty<bool>(KILL_ON_GROUND);

        public static readonly PropertyMeta<bool> POINT_TO_DIRECTION = new PropertyMeta<bool>("pointToDirection");
        public static bool PointsTowardDirection(this Entity entity)
        {
            return entity.GetProperty<bool>(POINT_TO_DIRECTION);
        }
        public static readonly PropertyMeta<bool> NO_DESTROY_OUTSIDE_LAWN = new PropertyMeta<bool>("noDestroyOutsideLawn");
        public static bool WillDestroyOutsideLawn(this Entity projectile)
        {
            return !projectile.GetProperty<bool>(NO_DESTROY_OUTSIDE_LAWN);
        }
        public static readonly PropertyMeta<bool> PIERCING = new PropertyMeta<bool>("piercing");
        public static bool IsPiercing(this Entity projectile)
        {
            return projectile.GetProperty<bool>(PIERCING);
        }
        public static void SetPiercing(this Entity projectile, bool value)
        {
            projectile.SetProperty(PIERCING, value);
        }
        public static readonly PropertyMeta<NamespaceID[]> DAMAGE_EFFECTS = new PropertyMeta<NamespaceID[]>("damageEffects");
        public static NamespaceID[] GetDamageEffects(this Entity projectile)
        {
            return projectile.GetProperty<NamespaceID[]>(DAMAGE_EFFECTS);
        }
        public static readonly PropertyMeta<bool> NO_HIT_ENTITIES = new PropertyMeta<bool>("noHitEntities");
        public static bool DontHitEntities(this Entity projectile)
        {
            return projectile.GetProperty<bool>(NO_HIT_ENTITIES);
        }
        public static readonly PropertyMeta<List<EntityColliderReference>> IGNORED_COLLIDERS = new PropertyMeta<List<EntityColliderReference>>("ignoredColliders");
        public static void AddIgnoredProjectileCollider(this Entity projectile, IEntityCollider other)
        {
            var colliders = projectile.GetBehaviourField<List<EntityColliderReference>>(IGNORED_COLLIDERS);
            if (colliders == null)
            {
                colliders = new List<EntityColliderReference>();
                projectile.SetBehaviourField(IGNORED_COLLIDERS, colliders);
            }
            var reference = other.ToReference();
            if (colliders.Contains(reference))
                return;
            colliders.Add(reference);
        }
        public static void RemoveIgnoredProjectileCollider(this Entity projectile, IEntityCollider other)
        {
            var colliders = projectile.GetBehaviourField<List<EntityColliderReference>>(IGNORED_COLLIDERS);
            if (colliders == null)
            {
                return;
            }
            var reference = other.ToReference();
            colliders.Remove(reference);
        }
        public static bool IsProjectileColliderIgnored(this Entity projectile, IEntityCollider other)
        {
            var colliders = projectile.GetBehaviourField<List<EntityColliderReference>>(IGNORED_COLLIDERS);
            if (colliders == null)
            {
                return false;
            }
            var reference = other.ToReference();
            return colliders.Contains(reference);
        }
        public static void ClearIgnoredProjectileColliders(this Entity projectile)
        {
            var colliders = projectile.GetBehaviourField<List<EntityColliderReference>>(IGNORED_COLLIDERS);
            if (colliders == null)
            {
                return;
            }
            colliders.Clear();
        }
    }
}
