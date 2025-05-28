using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Carts
{
    public static class VanillaCartNames
    {
        public const string minecart = "minecart";
        public const string pumpkinCarriage = "pumpkin_carriage";
        public const string nyanCat = "nyan_cat";
        public const string nyaightmare = "nyaightmare";
        public const string bowlChariot = "bowl_chariot";
        public const string ballista = "ballista";
    }
    public static class VanillaCartID
    {
        public static readonly NamespaceID minecart = Get(VanillaCartNames.minecart);
        public static readonly NamespaceID pumpkinCarriage = Get(VanillaCartNames.pumpkinCarriage);
        public static readonly NamespaceID nyanCat = Get(VanillaCartNames.nyanCat);
        public static readonly NamespaceID nyaightmare = Get(VanillaCartNames.nyaightmare);
        public static readonly NamespaceID bowlChariot = Get(VanillaCartNames.bowlChariot);
        public static readonly NamespaceID ballista = Get(VanillaCartNames.ballista);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
