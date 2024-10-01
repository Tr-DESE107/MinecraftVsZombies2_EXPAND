using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class PickupNames
    {
        public const string redstone = "redstone";
        public const string emerald = "emerald";
        public const string ruby = "ruby";
        public const string diamond = "diamond";
        public const string clearPickup = "clear_pickup";
        public const string starshard = "starshard";
    }
    public static class PickupID
    {
        public static readonly NamespaceID redstone = Get(PickupNames.redstone);
        public static readonly NamespaceID emerald = Get(PickupNames.emerald);
        public static readonly NamespaceID ruby = Get(PickupNames.ruby);
        public static readonly NamespaceID diamond = Get(PickupNames.diamond);
        public static readonly NamespaceID clearPickup = Get(PickupNames.clearPickup);
        public static readonly NamespaceID starshard = Get(PickupNames.starshard);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
