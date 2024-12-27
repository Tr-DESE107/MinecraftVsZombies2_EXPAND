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
        public const string rain = "rain";
        public const string shineRing = "shine_ring";
        public const string stunStars = "stun_stars";
        public const string stunningFlash = "stunning_flash";
        public const string explosion = "explosion";
        public const string soulfire = "soulfire";
        public const string soulfireBurn = "soulfire_burn";
        public const string soulfireBlast = "soulfire_blast";
        public const string mummyGas = "mummy_gas";
        public const string burningGas = "burning_gas";
        public const string healParticles = "heal_particles";
        public const string boneParticles = "bone_particles";
        public const string bloodParticles = "blood_particles";
        public const string smokeCluster = "smoke_cluster";
        public const string electricArc = "electric_arc";
        public const string goreParticles = "gore_particles";
        public const string frankensteinJumpTrail = "frankenstein_jump_trail";
        public const string frankensteinHead = "frankenstein_head";
        public const string splashParticles = "splash_particles";
        public const string gearParticles = "gear_particles";
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
        public static readonly NamespaceID explosion = Get(VanillaEffectNames.explosion);
        public static readonly NamespaceID rain = Get(VanillaEffectNames.rain);
        public static readonly NamespaceID shineRing = Get(VanillaEffectNames.shineRing);
        public static readonly NamespaceID stunStars = Get(VanillaEffectNames.stunStars);
        public static readonly NamespaceID stunningFlash = Get(VanillaEffectNames.stunningFlash);
        public static readonly NamespaceID soulfire = Get(VanillaEffectNames.soulfire);
        public static readonly NamespaceID soulfireBurn = Get(VanillaEffectNames.soulfireBurn);
        public static readonly NamespaceID soulfireBlast = Get(VanillaEffectNames.soulfireBlast);
        public static readonly NamespaceID mummyGas = Get(VanillaEffectNames.mummyGas);
        public static readonly NamespaceID burningGas = Get(VanillaEffectNames.burningGas);
        public static readonly NamespaceID healParticles = Get(VanillaEffectNames.healParticles);
        public static readonly NamespaceID boneParticles = Get(VanillaEffectNames.boneParticles);
        public static readonly NamespaceID bloodParticles = Get(VanillaEffectNames.bloodParticles);
        public static readonly NamespaceID smokeCluster = Get(VanillaEffectNames.smokeCluster);
        public static readonly NamespaceID electricArc = Get(VanillaEffectNames.electricArc);
        public static readonly NamespaceID gore = Get(VanillaEffectNames.goreParticles);
        public static readonly NamespaceID frankensteinJumpTrail = Get(VanillaEffectNames.frankensteinJumpTrail);
        public static readonly NamespaceID frankensteinHead = Get(VanillaEffectNames.frankensteinHead);
        public static readonly NamespaceID splashParticles = Get(VanillaEffectNames.splashParticles);
        public static readonly NamespaceID gearParticles = Get(VanillaEffectNames.gearParticles);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
