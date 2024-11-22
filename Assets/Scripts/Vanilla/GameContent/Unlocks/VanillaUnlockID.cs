using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaUnlockID
    {
        public static readonly NamespaceID almanac = Get("almanac");
        public static readonly NamespaceID store = Get("store");
        public static readonly NamespaceID trigger = Get("trigger");
        public static readonly NamespaceID starshard = Get("starshard");
        public static readonly NamespaceID money = Get("money");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
