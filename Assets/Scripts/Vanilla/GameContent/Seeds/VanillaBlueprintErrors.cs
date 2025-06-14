using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Seeds
{
    public static class VanillaBlueprintErrors
    {
        public static readonly NamespaceID invalid = Get("invalid");
        public static readonly NamespaceID recharging = Get("recharging");
        public static readonly NamespaceID notEnoughEnergy = Get("not_enough_energy");
        public static readonly NamespaceID tutorial = Get("tutorial");
        public static readonly NamespaceID decrepify = Get("decrepify");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
