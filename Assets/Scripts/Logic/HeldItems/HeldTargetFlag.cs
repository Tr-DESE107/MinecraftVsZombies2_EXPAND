using System;

namespace MVZ2Logic.HeldItems
{
    [Flags]
    public enum HeldTargetFlag
    {
        None = 0,
        Plant = 1,
        Enemy = 1 << 1,
        Obstacle = 1 << 2,
        Boss = 1 << 3,
        Cart = 1 << 4,
        Pickup = 1 << 5,
        Projectile = 1 << 6,
        Effect = 1 << 7,
    }
}
