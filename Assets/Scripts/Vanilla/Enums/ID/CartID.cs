using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class CartNames
    {
        public const string minecart = "minecart";
    }
    public static class CartID
    {
        public static readonly NamespaceID minecart = Get(CartNames.minecart);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
