using System;
using System.Collections.Generic;
using PVZEngine.Entities;

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
    public static class HeldTargetFlagHelper
    {
        public static HeldTargetFlag GetHeldTargetFlagByType(int type)
        {
            return typeFlagDict.TryGetValue(type, out var flag) ? flag : HeldTargetFlag.None;
        }
        private static Dictionary<int, HeldTargetFlag> typeFlagDict = new Dictionary<int, HeldTargetFlag>()
        {
            { EntityTypes.PLANT, HeldTargetFlag.Plant },
            { EntityTypes.ENEMY, HeldTargetFlag.Enemy },
            { EntityTypes.OBSTACLE, HeldTargetFlag.Obstacle },
            { EntityTypes.BOSS, HeldTargetFlag.Boss },
            { EntityTypes.CART, HeldTargetFlag.Cart },
            { EntityTypes.PICKUP, HeldTargetFlag.Pickup },
            { EntityTypes.PROJECTILE, HeldTargetFlag.Projectile },
            { EntityTypes.EFFECT, HeldTargetFlag.Effect },
        };
    }
}
