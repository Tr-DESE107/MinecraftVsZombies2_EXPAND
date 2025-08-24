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
        public static readonly PropertyMeta<string> NAME = Get<string>("name");
        public static string GetOptionName(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<string>(NAME);
        }
        public static readonly PropertyMeta<SpriteReference> ICON = Get<SpriteReference>("icon");
        public static SpriteReference GetIcon(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<SpriteReference>(ICON);
        }
        public static readonly PropertyMeta<SpriteReference> MOBILE_ICON = Get<SpriteReference>("mobile_icon");
        public static SpriteReference GetMobileIcon(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<SpriteReference>(MOBILE_ICON);
        }

        public static readonly PropertyMeta<NamespaceID> MODEL_ID = Get<NamespaceID>("model_id");
        public static NamespaceID GetModelID(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(MODEL_ID);
        }

        public static readonly PropertyMeta<int> COST = Get<int>("cost");
        public static int GetCost(this SeedOptionDefinition definition)
        {
            return definition.GetProperty<int>(COST);
        }
    }
}
