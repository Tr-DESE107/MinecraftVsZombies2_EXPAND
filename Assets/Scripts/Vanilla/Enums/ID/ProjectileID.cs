using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ProjectileNames
    {
        public const string arrow = "arrow";
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
        public const string giantSpike = "giant_spike";
        public const string parabot = "parabot";
        public const string fireCharge = "fire_charge";
        public const string dart = "dart";
        public const string poisonJavelin = "poison_javelin";
        public const string weaknessGas = "weakness_gas";
    }
    public static class ProjectileID
    {
        public static readonly NamespaceID arrow = Get(ProjectileNames.arrow);
        public static readonly NamespaceID mineTNTSeed = Get(ProjectileNames.mineTNTSeed);
        public static readonly NamespaceID snowball = Get(ProjectileNames.snowball);
        public static readonly NamespaceID largeSnowball = Get(ProjectileNames.largeSnowball);
        public static readonly NamespaceID flyingTNT = Get(ProjectileNames.flyingTNT);
        public static readonly NamespaceID soulfireBall = Get(ProjectileNames.soulfireBall);
        public static readonly NamespaceID spiceGas = Get(ProjectileNames.spiceGas);
        public static readonly NamespaceID knife = Get(ProjectileNames.knife);
        public static readonly NamespaceID bullet = Get(ProjectileNames.bullet);
        public static readonly NamespaceID missile = Get(ProjectileNames.missile);

        public static readonly NamespaceID largeArrow = Get(ProjectileNames.largeArrow);
        public static readonly NamespaceID breakoutPearl = Get(ProjectileNames.breakoutPearl);
        public static readonly NamespaceID spike = Get(ProjectileNames.spike);
        public static readonly NamespaceID spikeBall = Get(ProjectileNames.spikeBall);
        public static readonly NamespaceID giantSpike = Get(ProjectileNames.giantSpike);
        public static readonly NamespaceID parabot = Get(ProjectileNames.parabot);
        public static readonly NamespaceID fireCharge = Get(ProjectileNames.fireCharge);
        public static readonly NamespaceID dart = Get(ProjectileNames.dart);
        public static readonly NamespaceID poisonJavelin = Get(ProjectileNames.poisonJavelin);
        public static readonly NamespaceID weaknessGas = Get(ProjectileNames.weaknessGas);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
