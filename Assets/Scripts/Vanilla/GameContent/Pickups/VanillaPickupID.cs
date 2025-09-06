using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Pickups
{
    public static class VanillaPickupNames
    {
        public const string redstone = "redstone";
        public const string gunpowder = "gunpowder";
        public const string furiousGunpowder = "furious_gunpowder";
        public const string emerald = "emerald";
        public const string ruby = "ruby";
        public const string sapphire = "sapphire";
        public const string diamond = "diamond";
        public const string clearPickup = "clear_pickup";
        public const string starshard = "starshard";
        public const string artifactPickup = "artifact_pickup";
        public const string blueprintPickup = "blueprint_pickup";

        public const string CopperIngot = "CopperIngot";
        public const string CopperNugget = "CopperNugget";
    }
    public static class VanillaPickupID
    {
        public static readonly NamespaceID redstone = Get(VanillaPickupNames.redstone);
        public static readonly NamespaceID gunpowder = Get(VanillaPickupNames.gunpowder);
        public static readonly NamespaceID furiousGunpowder = Get(VanillaPickupNames.furiousGunpowder);
        public static readonly NamespaceID emerald = Get(VanillaPickupNames.emerald);
        public static readonly NamespaceID ruby = Get(VanillaPickupNames.ruby);
        public static readonly NamespaceID sapphire = Get(VanillaPickupNames.sapphire);
        public static readonly NamespaceID diamond = Get(VanillaPickupNames.diamond);
        public static readonly NamespaceID clearPickup = Get(VanillaPickupNames.clearPickup);
        public static readonly NamespaceID starshard = Get(VanillaPickupNames.starshard);
        public static readonly NamespaceID artifactPickup = Get(VanillaPickupNames.artifactPickup);
        public static readonly NamespaceID blueprintPickup = Get(VanillaPickupNames.blueprintPickup);

        public static readonly NamespaceID CopperIngot = Get(VanillaPickupNames.CopperIngot);
        public static readonly NamespaceID CopperNugget = Get(VanillaPickupNames.CopperNugget);

        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
