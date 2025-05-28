﻿using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaPickupExt
    {
        public static void Collect(this Entity pickup)
        {
            if (!CanCollect(pickup))
                return;
            var collectible = pickup.Definition.GetBehaviour<ICollectiblePickup>();
            if (collectible != null)
                collectible.PostCollect(pickup);
        }
        public static bool CanCollect(this Entity entity)
        {
            var collectible = entity.Definition.GetBehaviour<ICollectiblePickup>();
            if (collectible == null)
                return false;
            if (!collectible.CanCollect(entity))
                return false;
            var result = new CallbackResult(true);
            entity.Level.Triggers.RunCallbackWithResult(VanillaLevelCallbacks.CAN_PICKUP_COLLECT, new EntityCallbackParams(entity), result);
            return result.GetValue<bool>();
        }
        public static bool IsCollected(this Entity entity)
        {
            return entity.State == VanillaEntityStates.PICKUP_COLLECTED;
        }
        public static Entity Produce(this Entity entity, NamespaceID pickupID, SpawnParams param = null)
        {
            return entity.Produce(entity.Level.Content.GetEntityDefinition(pickupID), param);
        }
        public static Entity Produce(this Entity entity, EntityDefinition pickupDef, SpawnParams param = null)
        {
            return entity.Level.Produce(pickupDef, entity.Position, entity, param);
        }
        public static Entity Produce(this LevelEngine level, NamespaceID pickupID, Vector3 position, Entity spawner, SpawnParams param = null)
        {
            return level.Produce(level.Content.GetEntityDefinition(pickupID), position, spawner, param);
        }
        public static Entity Produce(this LevelEngine level, EntityDefinition pickupDef, Vector3 position, Entity spawner, SpawnParams param = null)
        {
            float xSpeed;
            float maxSpeed = 1.6f;
            var pickup = level.Spawn(pickupDef, position, spawner, param);

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
            Vector3 dropVelocity = new Vector3(xSpeed, 7, 0);
            pickup.Velocity = dropVelocity;
            return pickup;
        }
    }
}
