namespace PVZEngine
{
    public static class EntityCollision
    {
        public const int MASK_PLANT = 1 << 0;
        public const int MASK_ENEMY = 1 << 1;
        public const int MASK_OBSTACLE = 1 << 2;
        public const int MASK_BOSS = 1 << 3;
        public const int MASK_CART = 1 << 4;
        public const int MASK_PICKUP = 1 << 5;
        public const int MASK_PROJECTILE = 1 << 6;
        public const int MASK_EFFECT = 1 << 7;
        public const int MASK_FRIENDLY = 1 << 8;
        public const int MASK_HOSTILE = 1 << 9;
        public const int MASK_ANY_FACTION = MASK_FRIENDLY | MASK_HOSTILE;

        public const int STATE_ENTER = 0;
        public const int STATE_STAY = 1;
        public const int STATE_EXIT = 2;
        public static bool CanCollide(Entity self, Entity other)
        {
            var mask = self.CollisionMask;
            if (self.IsEnemy(other))
            {
                if ((mask & MASK_HOSTILE) <= 0)
                    return false;
            }
            else
            {
                if ((mask & MASK_FRIENDLY) <= 0)
                    return false;
            }
            int typeMask = 0;
            switch (other.Type)
            {
                case EntityTypes.PLANT:
                    typeMask = MASK_PLANT;
                    break;
                case EntityTypes.ENEMY:
                    typeMask = MASK_ENEMY;
                    break;
                case EntityTypes.OBSTACLE:
                    typeMask = MASK_OBSTACLE;
                    break;
                case EntityTypes.BOSS:
                    typeMask = MASK_BOSS;
                    break;
                case EntityTypes.CART:
                    typeMask = MASK_CART;
                    break;
                case EntityTypes.PICKUP:
                    typeMask = MASK_PICKUP;
                    break;
                case EntityTypes.PROJECTILE:
                    typeMask = MASK_PROJECTILE;
                    break;
                case EntityTypes.EFFECT:
                    typeMask = MASK_EFFECT;
                    break;
            }
            if ((mask & typeMask) <= 0)
                return false;
            return true;
        }
    }
}
