using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Stages
{
    public static class VanillaIZombieLayoutNames
    {
        public const string dispenserPunchton4 = "dispenser_punchton4";
        public const string highAndLow4 = "high_and_low4";
        public const string redAlert5 = "red_alert5";
    }
    public static class VanillaIZombieLayoutID
    {
        public static readonly NamespaceID dispenserPunchton4 = Get(VanillaIZombieLayoutNames.dispenserPunchton4);
        public static readonly NamespaceID highAndLow4 = Get(VanillaIZombieLayoutNames.highAndLow4);
        public static readonly NamespaceID redAlert5 = Get(VanillaIZombieLayoutNames.redAlert5);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
