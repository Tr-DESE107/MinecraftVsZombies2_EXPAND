using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class VanillaBuffNames
    {
        public const string randomEnemySpeed = "random_enemy_speed";
        public const string armorDamageColor = "armor_damage_color";
        public const string damageColor = "damage_color";
        public const string productionColor = "production_color";
        public const string mineTNTInvincible = "mine_tnt_invincible";
        public const string obsidianArmor = "obsidianArmor";
        public const string starshardCarrier = "starshard_carrier";

        public static class SeedPack
        {
            public const string tutorialDisable = "tutorial_disable";
        }
        public static class Level
        {
            public const string tutorialPickaxeDisable = "tutorial_pickaxe_disable";
        }
    }
    public static class VanillaBuffID
    {
        public static readonly NamespaceID randomEnemySpeed = Get(VanillaBuffNames.randomEnemySpeed);
        public static readonly NamespaceID armorDamageColor = Get(VanillaBuffNames.armorDamageColor);
        public static readonly NamespaceID damageColor = Get(VanillaBuffNames.damageColor);
        public static readonly NamespaceID productionColor = Get(VanillaBuffNames.productionColor);
        public static readonly NamespaceID mineTNTInvincible = Get(VanillaBuffNames.mineTNTInvincible);
        public static readonly NamespaceID obsidianArmor = Get(VanillaBuffNames.obsidianArmor);
        public static readonly NamespaceID starshardCarrier = Get(VanillaBuffNames.starshardCarrier);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
