using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Seeds
{
    public static class VanillaBlueprintNames
    {
        public const string returnPearl = "return_pearl";
        public const string lengthenBoard = "lengthen_board";
        public const string addPearl = "add_pearl";
    }
    public static class VanillaBlueprintID
    {
        public static readonly NamespaceID returnPearl = Get(VanillaBlueprintNames.returnPearl);
        public static readonly NamespaceID lengthenBoard = Get(VanillaBlueprintNames.lengthenBoard);
        public static readonly NamespaceID addPearl = Get(VanillaBlueprintNames.addPearl);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
