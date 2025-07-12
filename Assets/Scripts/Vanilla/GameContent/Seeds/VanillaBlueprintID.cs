using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Seeds
{
    public static class VanillaBlueprintNames
    {
        public const string returnPearl = "return_pearl";
        public const string lengthenBoard = "lengthen_board";
        public const string addPearl = "add_pearl";

        public const string undeadFlyingObjectRed = "undead_flying_object_red";
        public const string undeadFlyingObjectGreen = "undead_flying_object_green";
        public const string undeadFlyingObjectBlue = "undead_flying_object_blue";
        public const string undeadFlyingObjectRainbow = "undead_flying_object_rainbow";
        public const string ufoRed = undeadFlyingObjectRed;
        public const string ufoGreen = undeadFlyingObjectGreen;
        public const string ufoBlue = undeadFlyingObjectBlue;
        public const string ufoRainbow = undeadFlyingObjectRainbow;
    }
    public static class VanillaBlueprintID
    {
        public static readonly NamespaceID returnPearl = Get(VanillaBlueprintNames.returnPearl);
        public static readonly NamespaceID lengthenBoard = Get(VanillaBlueprintNames.lengthenBoard);
        public static readonly NamespaceID addPearl = Get(VanillaBlueprintNames.addPearl);

        public static readonly NamespaceID undeadFlyingObjectRed = Get(VanillaBlueprintNames.undeadFlyingObjectRed);
        public static readonly NamespaceID undeadFlyingObjectGreen = Get(VanillaBlueprintNames.undeadFlyingObjectGreen);
        public static readonly NamespaceID undeadFlyingObjectBlue = Get(VanillaBlueprintNames.undeadFlyingObjectBlue);
        public static readonly NamespaceID undeadFlyingObjectRainbow = Get(VanillaBlueprintNames.undeadFlyingObjectRainbow);
        public static readonly NamespaceID ufoRed = Get(VanillaBlueprintNames.ufoRed);
        public static readonly NamespaceID ufoGreen = Get(VanillaBlueprintNames.ufoGreen);
        public static readonly NamespaceID ufoBlue = Get(VanillaBlueprintNames.ufoBlue);
        public static readonly NamespaceID ufoRainbow = Get(VanillaBlueprintNames.ufoRainbow);
        public static NamespaceID FromEntity(NamespaceID entityID)
        {
            return entityID;
        }
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
