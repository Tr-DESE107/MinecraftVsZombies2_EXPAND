using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Buffs
{
    public static class VanillaBuffNames
    {
        // Entities
        public const string entityPhysics = "entity_physics";

        // Contraption
        public const string productionColor = "production_color";
        public const string mineTNTInvincible = "mine_tnt_invincible";
        public const string obsidianArmor = "obsidian_armor";
        public const string moonlightSensorLaunching = "moonlight_sensor_launching";
        public const string moonlightSensorEvoked = "moonlight_sensor_evoked";
        public const string glowstoneEvoke = "glowstone_evoke";
        public const string tntIgnited = "tnt_ignited";
        public const string sacrificed = "sacrificed";
        public const string magichestInvincible = "magichest_invincible";
        // Enemy
        public const string starshardCarrier = "starshard_carrier";
        public const string randomEnemySpeed = "random_enemy_speed";
        public const string armorDamageColor = "armor_damage_color";
        public const string damageColor = "damage_color";
        public const string ghost = "ghost";
        public const string stun = "stun";
        // Obstacle
        public const string temporaryUpdateBeforeGame = "temporary_update_before_game";
        public const string thunderLightFadeout = "thunder_light_fadeout";

        // Projectile
        public const string projectileWait = "projectile_wait";

        // Cart
        public const string cartFadeIn = "cart_fade_in";

        // Difficulty
        public const string easyContraption = "easy_contraption";
        public const string easyArmor = "easy_armor";
        public const string hardEnemy = "hard_enemy";

        public static class SeedPack
        {
            public const string tutorialDisable = "tutorial_disable";
            public const string easyBlueprint = "easy_blueprint";
        }
        public static class Level
        {
            public const string levelEasy = "level_easy";
            public const string levelHard = "level_hard";
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
        public static readonly NamespaceID temporaryUpdateBeforeGame = Get(VanillaBuffNames.temporaryUpdateBeforeGame);
        public static readonly NamespaceID thunderLightFadeout = Get(VanillaBuffNames.thunderLightFadeout);
        public static readonly NamespaceID cartFadeIn = Get(VanillaBuffNames.cartFadeIn);
        public static readonly NamespaceID easyContraption = Get(VanillaBuffNames.easyContraption);
        public static readonly NamespaceID easyArmor = Get(VanillaBuffNames.easyArmor);
        public static readonly NamespaceID hardEnemy = Get(VanillaBuffNames.hardEnemy);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
        public static class SeedPack
        {
            public static readonly NamespaceID tutorialDisable = Get(VanillaBuffNames.SeedPack.tutorialDisable);
            public static readonly NamespaceID easyBlueprint = Get(VanillaBuffNames.SeedPack.easyBlueprint);
        }
        public static class Level
        {
            public static readonly NamespaceID levelEasy = Get(VanillaBuffNames.Level.levelEasy);
            public static readonly NamespaceID levelHard = Get(VanillaBuffNames.Level.levelHard);
            public static readonly NamespaceID tutorialPickaxeDisable = Get(VanillaBuffNames.Level.tutorialPickaxeDisable);
        }
    }
}
