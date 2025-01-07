using MVZ2Logic.SeedPacks;

namespace MVZ2Logic.SeedPacks
{
    public static class LogicSeedOptionProps
    {
        public const string ICON = "icon";
        public const string COST = "cost";
        public static SpriteReference GetIcon(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<SpriteReference>(ICON);
        }
        public static int GetCost(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<int>(COST);
        }
    }
}
