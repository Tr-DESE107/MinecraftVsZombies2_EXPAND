using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class VanillaPickupNames
    {
        public const string redstone = "redstone";
        public const string emerald = "emerald";
        public const string ruby = "ruby";
        public const string diamond = "diamond";
        public const string clearPickup = "clear_pickup";
        public const string starshard = "starshard";
    }
    public static class VanillaPickupID
    {
        public static readonly NamespaceID redstone = Get(VanillaPickupNames.redstone);
        public static readonly NamespaceID emerald = Get(VanillaPickupNames.emerald);
        public static readonly NamespaceID ruby = Get(VanillaPickupNames.ruby);
        public static readonly NamespaceID diamond = Get(VanillaPickupNames.diamond);
        public static readonly NamespaceID clearPickup = Get(VanillaPickupNames.clearPickup);
        public static readonly NamespaceID starshard = Get(VanillaPickupNames.starshard);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
