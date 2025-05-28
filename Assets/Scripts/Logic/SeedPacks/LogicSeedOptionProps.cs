using PVZEngine;

namespace MVZ2Logic.SeedPacks
{
    [PropertyRegistryRegion(LogicPropertyRegions.seedOption)]
    public static class LogicSeedOptionProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<SpriteReference> ICON = Get<SpriteReference>("icon");
        public static SpriteReference GetIcon(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<SpriteReference>(ICON);
        }
        public static readonly PropertyMeta<int> COST = Get<int>("cost");
        public static int GetCost(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<int>(COST);
        }
    }
}
