using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaUnlockID
    {
        public static readonly NamespaceID money = Get("money");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
