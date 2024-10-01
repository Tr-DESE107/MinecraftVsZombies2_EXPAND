using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Extensions
{
    public static class MVZ2Pickup
    {
        public static Entity Produce(this Entity entity, NamespaceID pickupID)
        {
            return entity.Produce(entity.Level.ContentProvider.GetEntityDefinition(pickupID));
        }
        public static Entity Produce<T>(this Entity entity) where T : EntityDefinition
        {
            return entity.Produce(entity.Level.ContentProvider.GetEntityDefinition<T>());
        }
        public static Entity Produce(this Entity entity, EntityDefinition pickupDef)
        {
            return entity.Level.Produce(pickupDef, entity.Position, entity);
        }
        public static Entity Produce(this LevelEngine level, NamespaceID pickupID, Vector3 position, Entity spawner)
        {
            return level.Produce(level.ContentProvider.GetEntityDefinition(pickupID), position, spawner);
        }
        public static Entity Produce<T>(this LevelEngine level, Vector3 position, Entity spawner) where T : EntityDefinition
        {
            return level.Produce(level.ContentProvider.GetEntityDefinition<T>(), position, spawner);
        }
        public static Entity Produce(this LevelEngine level, EntityDefinition pickupDef, Vector3 position, Entity spawner)
        {
            float xSpeed;
            float maxSpeed = 1.6f;
            var pickup = level.Spawn(pickupDef, position, spawner);

            var rng = pickup.RNG;
            if (position.x <= BuiltinLevel.GetBorderX(false) + 150)
            {
                xSpeed = rng.Next(0, maxSpeed);
            }
            else if (position.x >= BuiltinLevel.GetBorderX(true) - 150)
            {
                xSpeed = rng.Next(-maxSpeed, 0);
            }
            else
            {
                xSpeed = rng.Next(-maxSpeed, maxSpeed);
            }
            Vector3 dropVelocity = new Vector3(xSpeed, 14, 0);
            pickup.Velocity = dropVelocity;
            return pickup;
        }
    }
}
