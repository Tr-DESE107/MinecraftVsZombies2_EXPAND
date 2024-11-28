using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Effects
{
    public static class VanillaEffectNames
    {
        public const string miner = "miner";
        public const string mineDebris = "mine_debris";
        public const string brokenArmor = "broken_armor";
        public const string fragment = "fragment";
        public const string starParticles = "star_particles";
        public const string gemEffect = "gem_effect";
        public const string smoke = "smoke";
        public const string thunderBolt = "thunder_bolt";
        public const string evocationStar = "evocation_star";
        public const string shineRing = "shine_ring";
        public const string stunStars = "stun_stars";
        public const string stunningFlash = "stunning_flash";
        public const string explosion = "explosion";
        public const string soulfire = "soulfire";
        public const string soulfireBurn = "soulfire_burn";
        public const string soulfireBlast = "soulfire_blast";
    }
    public static class VanillaEffectID
    {
        public static readonly NamespaceID miner = Get(VanillaEffectNames.miner);
        public static readonly NamespaceID mineDebris = Get(VanillaEffectNames.mineDebris);
        public static readonly NamespaceID brokenArmor = Get(VanillaEffectNames.brokenArmor);
        public static readonly NamespaceID fragment = Get(VanillaEffectNames.fragment);
        public static readonly NamespaceID starParticles = Get(VanillaEffectNames.starParticles);
        public static readonly NamespaceID gemEffect = Get(VanillaEffectNames.gemEffect);
        public static readonly NamespaceID smoke = Get(VanillaEffectNames.smoke);
        public static readonly NamespaceID thunderBolt = Get(VanillaEffectNames.thunderBolt);
        public static readonly NamespaceID evocationStar = Get(VanillaEffectNames.evocationStar);
        public static readonly NamespaceID shineRing = Get(VanillaEffectNames.shineRing);
        public static readonly NamespaceID stunStars = Get(VanillaEffectNames.stunStars);
        public static readonly NamespaceID stunningFlash = Get(VanillaEffectNames.stunningFlash);
        public static readonly NamespaceID explosion = Get(VanillaEffectNames.explosion);
        public static readonly NamespaceID soulfire = Get(VanillaEffectNames.soulfire);
        public static readonly NamespaceID soulfireBurn = Get(VanillaEffectNames.soulfireBurn);
        public static readonly NamespaceID soulfireBlast = Get(VanillaEffectNames.soulfireBlast); 
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
