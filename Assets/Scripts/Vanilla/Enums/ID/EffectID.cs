using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class EffectNames
    {
        public const string miner = "miner";
        public const string mineDebris = "mine_debris";
    }
    public static class EffectID
    {
        public static readonly NamespaceID miner = Get(EffectNames.miner);
        public static readonly NamespaceID mineDebris = Get(EffectNames.mineDebris);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
