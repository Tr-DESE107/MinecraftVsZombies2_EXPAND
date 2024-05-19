using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ProjectileID
    {
        public static readonly NamespaceID arrow = Get("arrow");
        public static readonly NamespaceID mineTNTSeed = Get("mine_tnt_seed");
        public static readonly NamespaceID snowball = Get("snowball");
        public static readonly NamespaceID largeSnowball = Get("large_snowball");
        public static readonly NamespaceID flyingTNT = Get("flying_tnt");
        public static readonly NamespaceID soulfireBall = Get("soulfire_ball");
        public static readonly NamespaceID spiceGas = Get("spice_gas");
        public static readonly NamespaceID knife = Get("knife");
        public static readonly NamespaceID bullet = Get("bullet");
        public static readonly NamespaceID missile = Get("missile");

        public static readonly NamespaceID largeArrow = Get("large_arrow");
        public static readonly NamespaceID breakoutPearl = Get("breakout_pearl");
        public static readonly NamespaceID spike = Get("spike");
        public static readonly NamespaceID spikeBall = Get("spike_ball");
        public static readonly NamespaceID giantSpike = Get("giant_spike");
        public static readonly NamespaceID parabot = Get("parabot");
        public static readonly NamespaceID fireCharge = Get("fire_charge");
        public static readonly NamespaceID dart = Get("dart");
        public static readonly NamespaceID poisonJavelin = Get("poison_javelin");
        public static readonly NamespaceID weaknessGas = Get("weakness_gas");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
