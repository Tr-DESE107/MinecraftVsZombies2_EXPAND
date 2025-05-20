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
        public const int ENEMY_RUN = 201;
        public const int ENEMY_SPECIAL = 202;
        public const int ENEMY_SIT = 203;

        public const int BOSS_APPEAR = 300;
        public const int BOSS_ATTACK_2 = 301;
        public const int BOSS_ATTACK_3 = 302;
        public const int BOSS_ATTACK_4 = 303;
        public const int BOSS_ATTACK_5 = 304;
        public const int BOSS_ATTACK_6 = 305;
        public const int BOSS_SPECIAL = 310;
        public const int BOSS_SPECIAL_2 = 311;
        public const int BOSS_SPECIAL_3 = 312;
        public const int BOSS_FAINT = 350;

        public const int CART_TRIGGERED = 500;

        public const int PICKUP_COLLECTED = 600;


        // Contraptions
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

        public const int TESLA_COIL_IDLE = IDLE;
        public const int TESLA_COIL_ATTACK = ATTACK;

        // Enemies
        public const int NECROMANCER_CAST = ENEMY_CAST;

        public const int SPIDER_CLIMB = ENEMY_CAST;

        public const int MESMERIZER_CAST = ENEMY_CAST;

        public const int SKELETON_HORSE_GALLOP = ENEMY_RUN;
        public const int SKELETON_HORSE_JUMP = ENEMY_CAST;
        public const int SKELETON_HORSE_LAND = ENEMY_SPECIAL;

        public const int EMPEROR_ZOMBIE_CAST = ENEMY_CAST;

        public const int MUTANT_ZOMBIE_IDLE = IDLE;
        public const int MUTANT_ZOMBIE_WALK = WALK;
        public const int MUTANT_ZOMBIE_ATTACK = ATTACK;
        public const int MUTANT_ZOMBIE_THROW = ENEMY_CAST;
        public const int MUTANT_ZOMBIE_DEATH = DEAD;

        // Bosses
        public const int FRANKENSTEIN_IDLE = IDLE;
        public const int FRANKENSTEIN_JUMP = WALK;
        public const int FRANKENSTEIN_GUN = ATTACK;
        public const int FRANKENSTEIN_DEAD = DEAD;
        public const int FRANKENSTEIN_MISSILE = BOSS_ATTACK_2;
        public const int FRANKENSTEIN_PUNCH = BOSS_ATTACK_3;
        public const int FRANKENSTEIN_SHOCK = BOSS_ATTACK_4;
        public const int FRANKENSTEIN_WAKING = BOSS_APPEAR;
        public const int FRANKENSTEIN_FAINT = BOSS_FAINT;

        public const int NIGHTMAREAPER_APPEAR = BOSS_APPEAR;
        public const int NIGHTMAREAPER_IDLE = IDLE;
        public const int NIGHTMAREAPER_JAB = ATTACK;
        public const int NIGHTMAREAPER_SPIN = BOSS_ATTACK_2;
        public const int NIGHTMAREAPER_DARKNESS = BOSS_ATTACK_3;
        public const int NIGHTMAREAPER_REVIVE = BOSS_ATTACK_4;
        public const int NIGHTMAREAPER_ENRAGE = BOSS_SPECIAL;
        public const int NIGHTMAREAPER_DEATH = DEAD;

        public const int SEIJA_APPEAR = BOSS_APPEAR;
        public const int SEIJA_IDLE = IDLE;
        public const int SEIJA_DANMAKU = ATTACK;
        public const int SEIJA_HAMMER = BOSS_ATTACK_2;
        public const int SEIJA_GAP_BOMB = BOSS_ATTACK_3;
        public const int SEIJA_CAMERA = BOSS_ATTACK_4;
        public const int SEIJA_BACKFLIP = BOSS_SPECIAL;
        public const int SEIJA_FRONTFLIP = BOSS_SPECIAL_2;
        public const int SEIJA_FABRIC = BOSS_SPECIAL_3;
        public const int SEIJA_FAINT = DEAD;

        public const int WITHER_APPEAR = BOSS_APPEAR;
        public const int WITHER_IDLE = IDLE;
        public const int WITHER_CHARGE = ATTACK;
        public const int WITHER_EAT = BOSS_ATTACK_2;
        public const int WITHER_SWITCH = BOSS_SPECIAL;
        public const int WITHER_SUMMON = BOSS_ATTACK_3;
        public const int WITHER_STUNNED = BOSS_FAINT;
        public const int WITHER_DEATH = DEAD;

        public const int THE_GIANT_IDLE = BOSS_APPEAR;
        public const int THE_GIANT_DISASSEMBLY = BOSS_SPECIAL;
        public const int THE_GIANT_EYES = ATTACK;
        public const int THE_GIANT_ROAR = BOSS_ATTACK_2;
        public const int THE_GIANT_ARMS = BOSS_ATTACK_3;
        public const int THE_GIANT_BREATH = BOSS_ATTACK_4;
        public const int THE_GIANT_STUNNED = BOSS_FAINT;
        public const int THE_GIANT_PACMAN = BOSS_ATTACK_5;
        public const int THE_GIANT_SNAKE = BOSS_ATTACK_6;
        public const int THE_GIANT_FAINT = BOSS_SPECIAL_2;
        public const int THE_GIANT_CHASE = WALK;
        public const int THE_GIANT_DEATH = DEAD;


        public const int BREAKOUT_PEARL_IDLE = IDLE;
        public const int BREAKOUT_PEARL_RETURN = WALK;
        public const int BREAKOUT_PEARL_FIRED = ATTACK;


        public const int HOE_IDLE = IDLE;
        public const int HOE_TRIGGERED = ATTACK;
        public const int HOE_DAMAGED = DEAD;

        public const int CRUSHING_WALLS_IDLE = IDLE;
        public const int CRUSHING_WALLS_ENRAGED = WALK;
        public const int CRUSHING_WALLS_CLOSED = ATTACK;
        public const int CRUSHING_WALLS_STOPPED = DEAD;
    }
}
