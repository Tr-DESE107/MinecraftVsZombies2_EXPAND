using MVZ2.GameContent.Seeds;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Seed
    {
        public static int GetSeedType(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(SeedProps.SEED_TYPE);
        }
        public static NamespaceID GetSeedEntityID(this SeedDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(SeedProps.SEED_ENTITY_ID);
        }
        public static int GetCost(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(SeedProperties.COST);
        }
        public static bool IsTriggerActive(this SeedDefinition definition)
        {
            return definition.GetProperty<bool>(SeedProps.TRIGGER_ACTIVE);
        }
        public static int GetTriggerCost(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(SeedProps.TRIGGER_COST);
        }
    }
}
