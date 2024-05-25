using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class EffectNames
    {
        public const string miner = "miner";
        public const string mineDebris = "mine_debris";
        public const string brokenArmor = "broken_armor";
    }
    public static class EffectID
    {
        public static readonly NamespaceID miner = Get(EffectNames.miner);
        public static readonly NamespaceID mineDebris = Get(EffectNames.mineDebris);
        public static readonly NamespaceID brokenArmor = Get(EffectNames.brokenArmor);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
