using MVZ2Logic;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.SeedPacks
{
    [PropertyRegistryRegion(PropertyRegions.seed)]
    public static class LogicSeedProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<SpriteReference> ICON = Get<SpriteReference>("icon");
        public static SpriteReference GetIcon(this SeedDefinition seed)
        {
            return seed.GetProperty<SpriteReference>(ICON);
        }
        public static SpriteReference GetIcon(this SeedPack seed)
        {
            return seed.GetProperty<SpriteReference>(ICON);
        }
        public static void SetIcon(this SeedDefinition seed, SpriteReference value)
        {
            seed.SetProperty(ICON, value);
        }
        public static readonly PropertyMeta<NamespaceID> MODEL_ID = Get<NamespaceID>("model_id");
        public static NamespaceID GetModelID(this SeedDefinition seed)
        {
            return seed.GetProperty<NamespaceID>(MODEL_ID);
        }
        public static NamespaceID GetModelID(this SeedPack seed)
        {
            return seed.GetProperty<NamespaceID>(MODEL_ID);
        }
        public static void SetModelID(this SeedDefinition seed, NamespaceID value)
        {
            seed.SetProperty(MODEL_ID, value);
        }
        public static readonly PropertyMeta<int> VARIANT = Get<int>("variant");
        public static int GetVariant(this SeedDefinition seed)
        {
            return seed.GetProperty<int>(VARIANT);
        }
        public static int GetVariant(this SeedPack seed)
        {
            return seed.GetProperty<int>(VARIANT);
        }
        public static void SetVariant(this SeedDefinition seed, int value)
        {
            seed.SetProperty(VARIANT, value);
        }
    }
}
