using MVZ2.Vanilla;
using System.Collections.Generic;
using PVZEngine.Level;
using PVZEngine;

namespace MVZ2.GameContent.Projectiles
{
    public static class VanillaProjectileProps
    {
        public const string PIERCING = "piercing";
        public const string POINT_TO_DIRECTION = "pointToDirection";
        public const string DAMAGE_EFFECTS = "damageEffects";
        public const string NO_DESTROY_OUTSIDE_LAWN = "noDestroyOutsideLawn";
        public const string CAN_HIT_SPAWNER = "canHitSpawner";
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
        public static NamespaceID[] GetDamageEffects(this Entity projectile)
        {
            return projectile.GetProperty<NamespaceID[]>(DAMAGE_EFFECTS);
        }
        public static bool CanHitSpawner(this Entity projectile)
        {
            return projectile.GetProperty<bool>(CAN_HIT_SPAWNER);
        }
        public static void SetCanHitSpawner(this Entity projectile, bool value)
        {
            projectile.SetProperty(CAN_HIT_SPAWNER, value);
        }
        public static List<EntityID> GetProjectileCollidingEntities(this Entity projectile)
        {
            return projectile.GetProperty<List<EntityID>>(COLLIDING_ENTITIES);
        }
        public static void SetProjectileCollidingEntities(this Entity projectile, List<EntityID> value)
        {
            projectile.SetProperty(COLLIDING_ENTITIES, value);
        }
        public static void AddProjectileCollidingEntity(this Entity projectile, Entity entity)
        {
            var entities = projectile.GetProjectileCollidingEntities();
            if (entities == null)
            {
                entities = new List<EntityID>();
                projectile.SetProjectileCollidingEntities(entities);
            }
            entities.Add(new EntityID(entity));
        }
        public static bool RemoveProjectileCollidingEntity(this Entity projectile, Entity entity)
        {
            var entities = projectile.GetProjectileCollidingEntities();
            if (entities == null)
                return false;
            return entities.RemoveAll(e => e.ID == entity.ID) > 0;
        }
        public static bool CanPierce(this Entity projectile, Entity other)
        {
            bool ethereal = Armor.Exists(other.EquipedArmor) ? false : other.IsEthereal();
            return ethereal || projectile.IsPiercing();
        }
    }
}
