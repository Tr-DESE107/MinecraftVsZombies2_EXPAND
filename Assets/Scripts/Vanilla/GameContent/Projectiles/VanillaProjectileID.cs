using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Projectiles
{
    public static class VanillaProjectileNames
    {
        public const string arrow = "arrow";
        public const string purpleArrow = "purpleArrow";
        public const string mineTNTSeed = "mine_tnt_seed";
        public const string snowball = "snowball";
        public const string largeSnowball = "large_snowball";
        public const string flyingTNT = "flying_tnt";
        public const string soulfireBall = "soulfire_ball";
        public const string spiceGas = "spice_gas";
        public const string knife = "knife";
        public const string bullet = "bullet";
        public const string missile = "missile";

        public const string largeArrow = "large_arrow";
        public const string breakoutPearl = "breakout_pearl";
        public const string spike = "spike";
        public const string spikeBall = "spike_ball";
        public const string parabot = "parabot";
        public const string fireCharge = "fire_charge";
        public const string dart = "dart";
        public const string poisonJavelin = "poison_javelin";
        public const string weaknessGas = "weakness_gas";

        public const string woodenBall = "wooden_ball";
        public const string cobble = "cobble";
        public const string boulder = "boulder";
        public const string goldenBall = "golden_ball";
        public const string diamondCaltrop = "diamond_caltrop";
        public const string compellingOrb = "compelling_orb";
        public const string seijaMagicBomb = "seija_magic_bomb";
        public const string seijaBullet = "seija_bullet";
        public const string witherSkull = "wither_skull";
    }
    public static class VanillaProjectileID
    {
        public static readonly NamespaceID arrow = Get(VanillaProjectileNames.arrow);
        public static readonly NamespaceID purpleArrow = Get(VanillaProjectileNames.purpleArrow);
        public static readonly NamespaceID mineTNTSeed = Get(VanillaProjectileNames.mineTNTSeed);
        public static readonly NamespaceID snowball = Get(VanillaProjectileNames.snowball);
        public static readonly NamespaceID largeSnowball = Get(VanillaProjectileNames.largeSnowball);
        public static readonly NamespaceID flyingTNT = Get(VanillaProjectileNames.flyingTNT);
        public static readonly NamespaceID soulfireBall = Get(VanillaProjectileNames.soulfireBall);
        public static readonly NamespaceID spiceGas = Get(VanillaProjectileNames.spiceGas);
        public static readonly NamespaceID knife = Get(VanillaProjectileNames.knife);
        public static readonly NamespaceID bullet = Get(VanillaProjectileNames.bullet);
        public static readonly NamespaceID missile = Get(VanillaProjectileNames.missile);

        public static readonly NamespaceID largeArrow = Get(VanillaProjectileNames.largeArrow);
        public static readonly NamespaceID breakoutPearl = Get(VanillaProjectileNames.breakoutPearl);
        public static readonly NamespaceID spike = Get(VanillaProjectileNames.spike);
        public static readonly NamespaceID spikeBall = Get(VanillaProjectileNames.spikeBall);
        public static readonly NamespaceID parabot = Get(VanillaProjectileNames.parabot);
        public static readonly NamespaceID fireCharge = Get(VanillaProjectileNames.fireCharge);
        public static readonly NamespaceID dart = Get(VanillaProjectileNames.dart);
        public static readonly NamespaceID poisonJavelin = Get(VanillaProjectileNames.poisonJavelin);
        public static readonly NamespaceID weaknessGas = Get(VanillaProjectileNames.weaknessGas);

        public static readonly NamespaceID woodenBall = Get(VanillaProjectileNames.woodenBall);
        public static readonly NamespaceID cobble = Get(VanillaProjectileNames.cobble);
        public static readonly NamespaceID boulder = Get(VanillaProjectileNames.boulder);
        public static readonly NamespaceID goldenBall = Get(VanillaProjectileNames.goldenBall);
        public static readonly NamespaceID diamondCaltrop = Get(VanillaProjectileNames.diamondCaltrop);
        public static readonly NamespaceID compellingOrb = Get(VanillaProjectileNames.compellingOrb);
        public static readonly NamespaceID seijaMagicBomb = Get(VanillaProjectileNames.seijaMagicBomb);
        public static readonly NamespaceID seijaBullet = Get(VanillaProjectileNames.seijaBullet);
        public static readonly NamespaceID witherSkull = Get(VanillaProjectileNames.witherSkull);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
