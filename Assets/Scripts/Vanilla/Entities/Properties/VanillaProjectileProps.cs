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
        public static readonly PropertyMeta PIERCING = new PropertyMeta("piercing");
        public static readonly PropertyMeta POINT_TO_DIRECTION = new PropertyMeta("pointToDirection");
        public static readonly PropertyMeta DAMAGE_EFFECTS = new PropertyMeta("damageEffects");
        public static readonly PropertyMeta NO_DESTROY_OUTSIDE_LAWN = new PropertyMeta("noDestroyOutsideLawn");
        public static readonly PropertyMeta COLLIDING_ENTITIES = new PropertyMeta("collidingEntities");
        public static readonly PropertyMeta NO_HIT_ENTITIES = new PropertyMeta("noHitEntities");

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
        public static List<EntityColliderReference> GetProjectileCollidingColliders(this Entity projectile)
        {
            return projectile.GetProperty<List<EntityColliderReference>>(COLLIDING_ENTITIES);
        }
        public static void SetProjectileCollidingEntities(this Entity projectile, List<EntityColliderReference> value)
        {
            projectile.SetProperty(COLLIDING_ENTITIES, value);
        }
        public static void AddProjectileCollidingEntity(this Entity projectile, EntityColliderReference reference)
        {
            var entities = projectile.GetProjectileCollidingColliders();
            if (entities == null)
            {
                entities = new List<EntityColliderReference>();
                projectile.SetProjectileCollidingEntities(entities);
            }
            entities.Add(reference);
        }
        public static bool RemoveProjectileCollidingEntity(this Entity projectile, EntityColliderReference reference)
        {
            var entities = projectile.GetProjectileCollidingColliders();
            if (entities == null)
                return false;
            return entities.RemoveAll(e => e == reference) > 0;
        }
    }
}
