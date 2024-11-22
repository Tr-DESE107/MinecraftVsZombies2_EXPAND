using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaPickupExt
    {
        public static void Collect(this Entity pickup)
        {
            if (!PreCollect(pickup))
                return;
            pickup.State = EntityStates.COLLECTED;
            if (pickup.Definition is ICollectiblePickup collectible)
                collectible.PostCollect(pickup);
        }
        private static bool PreCollect(Entity entity)
        {
            if (entity.Definition is ICollectiblePickup collectible)
            {
                if (collectible.PreCollect(entity) == false)
                {
                    return false;
                }
            }
            var game = Global.Game;
            var triggers = game.GetTriggers(VanillaLevelCallbacks.PRE_PICKUP_COLLECT);
            foreach (var trigger in triggers)
            {
                var result = trigger.Invoke(entity);
                if (result is bool boolValue && !boolValue)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsCollected(this Entity entity)
        {
            return entity.State == EntityStates.COLLECTED;
        }
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
            if (position.x <= VanillaLevelExt.GetBorderX(false) + 150)
            {
                xSpeed = rng.Next(0, maxSpeed);
            }
            else if (position.x >= VanillaLevelExt.GetBorderX(true) - 150)
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
