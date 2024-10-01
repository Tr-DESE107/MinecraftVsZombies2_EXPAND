using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
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
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
