using MVZ2.GameContent;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class BuiltinPickup
    {
        public static bool IsCollected(this Entity entity)
        {
            return entity.State == EntityStates.COLLECTED;
        }
        public static void Collect(this Entity pickup)
        {
            pickup.State = EntityStates.COLLECTED;
            if (pickup.Definition is ICollectiblePickup collectible)
                collectible.PostCollect(pickup);
        }
        public static void Produce<T>(this Entity entity) where T : EntityDefinition
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
            var redstone = level.Spawn<T>(position, entity);
            redstone.Velocity = dropVelocity;

            level.PlaySound(SoundID.throwSound, entity.Pos);
        }
        public static bool IsImportantPickup(this Entity entity)
        {
            return entity.GetProperty<bool>(BuiltinPickupProps.IMPORTANT);
        }
        public static int GetCollectedTime(this Entity entity)
        {
            return entity.GetProperty<int>(BuiltinPickupProps.COLLECTED_TIME);
        }
        public static void SetCollectedTime(this Entity entity, int value)
        {
            entity.SetProperty(BuiltinPickupProps.COLLECTED_TIME, value);
        }
        public static void AddPickupCollectedTime(this Entity entity, int value)
        {
            entity.SetCollectedTime(entity.GetCollectedTime() + value);
        }
        public static bool CanAutoCollect(this Entity pickup)
        {
            return !pickup.GetProperty<bool>(BuiltinPickupProps.NO_AUTO_COLLECT);
        }
    }
}
