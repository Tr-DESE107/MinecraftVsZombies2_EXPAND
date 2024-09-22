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
            float xSpeed;
            float maxSpeed = 1.6f;
            Vector3 position = entity.Pos;

            var level = entity.Level;
            var rng = entity.RNG;
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
            var pickup = level.Spawn(pickupDef, position, entity);
            pickup.Velocity = dropVelocity;

            level.PlaySound(SoundID.throwSound, entity.Pos);
            return pickup;
        }
    }
}
