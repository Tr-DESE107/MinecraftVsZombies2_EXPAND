using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaProjectileProps
    {
        public const string PIERCING = "piercing";
        public const string POINT_TO_DIRECTION = "pointToDirection";
        public const string DAMAGE_EFFECTS = "damageEffects";
        public const string NO_DESTROY_OUTSIDE_LAWN = "noDestroyOutsideLawn";
        public const string COLLIDING_ENTITIES = "collidingEntities";
        public const string NO_HIT_ENTITIES = "noHitEntities";

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
