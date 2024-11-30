using PVZEngine;

namespace MVZ2.Vanilla.Audios
{
    public static class VanillaSoundID
    {
        public readonly static NamespaceID armorUp = Get("armor_up");
        public readonly static NamespaceID awooga = Get("awooga");
        public readonly static NamespaceID boneHit = Get("bone_hit");
        public readonly static NamespaceID boneWallBuild = Get("bone_wall_build"); 
        public readonly static NamespaceID bow = Get("bow");
        public readonly static NamespaceID buzzer = Get("buzzer");
        public readonly static NamespaceID chime = Get("chime");
        public readonly static NamespaceID coin = Get("coin");
        public readonly static NamespaceID darkSkiesCast = Get("dark_skies_cast");
        public readonly static NamespaceID darkSkiesImpact = Get("dark_skies_impact");
        public readonly static NamespaceID diamond = Get("diamond");
        public readonly static NamespaceID dirtRise = Get("dirt_rise");
        public readonly static NamespaceID explosion = Get("explosion");
        public readonly static NamespaceID evocation = Get("evocation");
        public readonly static NamespaceID fastForward = Get("fast_forward");
        public readonly static NamespaceID fire = Get("fire");
        public readonly static NamespaceID finalWave = Get("final_wave");
        public readonly static NamespaceID fizz = Get("fizz");
        public readonly static NamespaceID fuse = Get("fuse");
        public readonly static NamespaceID glowstone = Get("glowstone");
        public readonly static NamespaceID grass = Get("grass");
        public readonly static NamespaceID grind = Get("grind");
        public readonly static NamespaceID hit = Get("hit");
        public readonly static NamespaceID horseAngry = Get("horse_angry");
        public readonly static NamespaceID hugeWave = Get("huge_wave");
        public readonly static NamespaceID leatherHit = Get("leather_hit");
        public readonly static NamespaceID loseMusic = Get("lose_music");
        public readonly static NamespaceID mineExplode = Get("mine_explode");
        public readonly static NamespaceID minecart = Get("minecart");
        public readonly static NamespaceID moneyFall = Get("money_fall");
        public readonly static NamespaceID odd = Get("odd");
        public readonly static NamespaceID paper = Get("paper");
        public readonly static NamespaceID pause = Get("pause");
        public readonly static NamespaceID points = Get("points");
        public readonly static NamespaceID poisonCast = Get("poison_cast");
        public readonly static NamespaceID pick = Get("pick");
        public readonly static NamespaceID pickaxe = Get("pickaxe");
        public readonly static NamespaceID potion = Get("potion");
        public readonly static NamespaceID punch = Get("punch");
        public readonly static NamespaceID refuel = Get("refuel");
        public readonly static NamespaceID reviveCast = Get("revive_cast");
        public readonly static NamespaceID scream = Get("scream");
        public readonly static NamespaceID screw = Get("screw");
        public readonly static NamespaceID shot = Get("shot");
        public readonly static NamespaceID siren = Get("siren");
        public readonly static NamespaceID slice = Get("slice");
        public readonly static NamespaceID shieldHit = Get("shield_hit");
        public readonly static NamespaceID skeletonCry = Get("skeleton_cry");
        public readonly static NamespaceID skeletonDeath = Get("skeleton_death");
        public readonly static NamespaceID slowDown = Get("slow_down");
        public readonly static NamespaceID sparkle = Get("sparkle");
        public readonly static NamespaceID splat = Get("splat");
        public readonly static NamespaceID spring = Get("spring");
        public readonly static NamespaceID starshardAppear = Get("starshard_appear");
        public readonly static NamespaceID starshardUse = Get("starshard_use");
        public readonly static NamespaceID stone = Get("stone");
        public readonly static NamespaceID stunned = Get("stunned");
        public readonly static NamespaceID tap = Get("tap");
        public readonly static NamespaceID thunder = Get("thunder");
        public readonly static NamespaceID throwSound = Get("throw");
        public readonly static NamespaceID travel = Get("travel");
        public readonly static NamespaceID winMusic = Get("win_music");
        public readonly static NamespaceID wood = Get("wood");
        public readonly static NamespaceID zombieCry = Get("zombie_cry");
        public readonly static NamespaceID zombieDeath = Get("zombie_death");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
