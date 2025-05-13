using MVZ2Logic.SeedPacks;
using PVZEngine;

namespace MVZ2Logic.SeedPacks
{
    [PropertyRegistryRegion(LogicPropertyRegions.seedOption)]
    public static class LogicSeedOptionProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta ICON = Get("icon");
        public static readonly PropertyMeta COST = Get("cost");
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
