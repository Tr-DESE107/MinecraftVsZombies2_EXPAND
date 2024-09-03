using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class TextID
    {
        public static class UI
        {
            public static readonly NamespaceID purchase = Get("purchase");
            public static readonly NamespaceID confirmPurchaseSeventhSlot = Get("confirm_purchase_seventh_slot");
            public static readonly NamespaceID confirmTutorial = Get("confirm_tutorial");
            public static readonly NamespaceID tutorial = Get("confirm_tutorial");
            public static readonly NamespaceID yes = Get("yes");
            public static readonly NamespaceID no = Get("no");
            private static NamespaceID Get(string name)
            {
                return new NamespaceID(VanillaMod.spaceName, $"ui.{name}");
            }
        }
    }
}
