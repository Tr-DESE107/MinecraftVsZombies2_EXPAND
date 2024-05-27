using System.Collections.Generic;
using MVZ2.GameContent.Projectiles;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Projectile
    {
        public static bool WillDestroyOutsideLawn(this Entity projectile)
        {
            return !projectile.GetProperty<bool>(ProjectileProps.NO_DESTROY_OUTSIDE_LAWN);
        }
        public static bool IsPiercing(this Entity projectile)
        {
            return projectile.GetProperty<bool>(ProjectileProps.PIERCING);
        }
        public static bool CanHitSpawner(this Entity projectile)
        {
            return projectile.GetProperty<bool>(ProjectileProps.CAN_HIT_SPAWNER);
        }
        public static void SetCanHitSpawner(this Entity projectile, bool value)
        {
            projectile.SetProperty(ProjectileProps.CAN_HIT_SPAWNER, value);
        }
        public static List<EntityID> GetProjectileCollidingEntities(this Entity projectile)
        {
            return projectile.GetProperty<List<EntityID>>(ProjectileProps.COLLIDING_ENTITIES);
        }
        public static void SetProjectileCollidingEntities(this Entity projectile, List<EntityID> value)
        {
            projectile.SetProperty(ProjectileProps.COLLIDING_ENTITIES, value);
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
