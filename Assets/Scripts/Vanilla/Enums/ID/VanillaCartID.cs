using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class VanillaCartNames
    {
        public const string minecart = "minecart";
        public const string pumpkinCarriage = "pumpkin_carriage";
    }
    public static class VanillaCartID
    {
        public static readonly NamespaceID minecart = Get(VanillaCartNames.minecart);
        public static readonly NamespaceID pumpkinCarriage = Get(VanillaCartNames.pumpkinCarriage);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
