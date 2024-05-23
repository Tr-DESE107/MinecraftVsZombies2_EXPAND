using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class PickupNames
    {
        public const string redstone = "redstone";
    }
    public static class PickupID
    {
        public static readonly NamespaceID redstone = Get(PickupNames.redstone);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
