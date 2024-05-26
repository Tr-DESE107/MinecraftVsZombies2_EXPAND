using MVZ2.GameContent;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Pickup
    {
        public static bool CanAutoCollect(this Pickup pickup)
        {
            return !pickup.GetProperty<bool>(PickupProperties.NO_AUTO_COLLECT);
        }
        public static void Collect(this Pickup pickup)
        {
            pickup.IsCollected = true;
            if (pickup.Definition is ICollectiblePickup collectible)
                collectible.PostCollect(pickup);
        }
        public static void Produce<T>(this Entity entity) where T : EntityDefinition
        {
            float xSpeed;
            float maxSpeed = 1.6f;
            Vector3 position = entity.Pos;

            var level = entity.Game;
            var rng = entity.RNG;
            if (position.x <= MVZ2Game.GetBorderX(false) + 150)
            {
                xSpeed = rng.Next(0, maxSpeed);
            }
            else if (position.x >= MVZ2Game.GetBorderX(true) - 150)
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
    }
}
