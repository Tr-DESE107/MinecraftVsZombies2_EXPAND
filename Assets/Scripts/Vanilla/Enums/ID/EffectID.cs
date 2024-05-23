using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class EffectNames
    {
        public const string miner = "miner";
    }
    public static class EffectID
    {
        public static readonly NamespaceID miner = Get(EffectNames.miner);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
