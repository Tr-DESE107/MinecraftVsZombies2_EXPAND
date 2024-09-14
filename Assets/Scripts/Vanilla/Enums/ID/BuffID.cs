using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class BuffNames
    {
        public const string randomEnemySpeed = "random_enemy_speed";
        public const string armorDamageColor = "armor_damage_color";
        public const string damageColor = "damage_color";
        public const string productionColor = "production_color";
        public const string mineTNTInvincible = "mine_tnt_invincible";
        public const string obsidianArmor = "obsidianArmor";

        public static class SeedPack
        {
            public const string tutorialDisable = "tutorial_disable";
        }
        public static class Level
        {
            public const string tutorialPickaxeDisable = "tutorial_disable";
        }
    }
    public static class BuffID
    {
        public static readonly NamespaceID randomEnemySpeed = Get(BuffNames.randomEnemySpeed);
        public static readonly NamespaceID armorDamageColor = Get(BuffNames.armorDamageColor);
        public static readonly NamespaceID damageColor = Get(BuffNames.damageColor);
        public static readonly NamespaceID productionColor = Get(BuffNames.productionColor);
        public static readonly NamespaceID mineTNTInvincible = Get(BuffNames.mineTNTInvincible);
        public static readonly NamespaceID obsidianArmor = Get(BuffNames.obsidianArmor);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
