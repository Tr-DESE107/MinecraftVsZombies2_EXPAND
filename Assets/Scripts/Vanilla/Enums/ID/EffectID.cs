using PVZEngine;

namespace MVZ2.GameContent
{
    public static class EffectID
    {
        public static readonly NamespaceID miner = Get("miner");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
