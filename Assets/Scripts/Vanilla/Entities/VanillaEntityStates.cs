using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2Logic.Entities;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaEntityStates
    {
        public const int IDLE = 0;
        public const int WALK = 1;
        public const int ATTACK = 2;
        public const int DEAD = 3;

        public const int CONTRAPTION_COOLDOWN = 101;
        public const int CONTRAPTION_SPECIAL = 102;

        public const int ENEMY_CAST = 200;

        public const int CART_TRIGGERED = 500;

        public const int PICKUP_COLLECTED = 600;

        public const int PUNCHTON_IDLE = IDLE;
        public const int PUNCHTON_PUNCH = ATTACK;
        public const int PUNCHTON_BROKEN = CONTRAPTION_COOLDOWN;

        public const int MAGICHEST_IDLE = IDLE;
        public const int MAGICHEST_OPEN = ATTACK;
        public const int MAGICHEST_EAT = CONTRAPTION_SPECIAL;
        public const int MAGICHEST_LOMS = CONTRAPTION_SPECIAL;
        public const int MAGICHEST_CLOSE = CONTRAPTION_COOLDOWN;

        public const int NECROMANCER_CAST = ENEMY_CAST;
    }
}
