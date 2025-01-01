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

        public const int BOSS_APPEAR = 300;
        public const int BOSS_ATTACK_2 = 301;
        public const int BOSS_ATTACK_3 = 302;
        public const int BOSS_ATTACK_4 = 303;
        public const int BOSS_FAINT = 304;

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

        public const int VORTEX_HOPPER_SPIN = ATTACK;

        public const int PISTENSER_SEED = CONTRAPTION_SPECIAL;

        public const int TOTENSER_FIRE_BREATH = ATTACK;

        public const int NECROMANCER_CAST = ENEMY_CAST;

        public const int SPIDER_CLIMB = ENEMY_CAST;

        public const int FRANKENSTEIN_IDLE = IDLE;
        public const int FRANKENSTEIN_JUMP = WALK;
        public const int FRANKENSTEIN_GUN = ATTACK;
        public const int FRANKENSTEIN_DEAD = DEAD;
        public const int FRANKENSTEIN_MISSILE = BOSS_ATTACK_2;
        public const int FRANKENSTEIN_PUNCH = BOSS_ATTACK_3;
        public const int FRANKENSTEIN_SHOCK = BOSS_ATTACK_4;
        public const int FRANKENSTEIN_WAKING = BOSS_APPEAR;
        public const int FRANKENSTEIN_FAINT = BOSS_FAINT;

    }
}
