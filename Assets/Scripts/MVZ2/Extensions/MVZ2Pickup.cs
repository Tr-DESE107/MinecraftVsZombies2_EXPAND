using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2
{
    public static class MVZ2Pickup
    {
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
    }
}
