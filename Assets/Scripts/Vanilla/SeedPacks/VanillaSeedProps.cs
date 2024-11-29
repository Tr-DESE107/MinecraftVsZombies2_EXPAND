using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.SeedPacks
{
    public static class VanillaSeedProps
    {
        public const string SEED_ENTITY_ID = "seedEntityID";
        public const string SEED_TYPE = "seedType";
        public const string TWINKLING = "twinkling";
        public static int GetSeedType(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(SEED_TYPE);
        }
        public static NamespaceID GetSeedEntityID(this SeedDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(SEED_ENTITY_ID);
        }
        public static bool IsTwinkling(this SeedPack seed)
        {
            return seed.GetProperty<bool>(TWINKLING);
        }
        public static void SetTwinkling(this SeedPack seed, bool value)
        {
            seed.SetProperty(TWINKLING, value);
        }
    }
}
