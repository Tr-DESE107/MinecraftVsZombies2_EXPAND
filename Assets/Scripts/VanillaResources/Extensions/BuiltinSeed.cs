using PVZEngine;
using PVZEngine.Definitions;

namespace MVZ2.GameContent
{
    public static class BuiltinSeed
    {
        public static int GetSeedType(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(BuiltinSeedProps.SEED_TYPE);
        }
        public static NamespaceID GetSeedEntityID(this SeedDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(BuiltinSeedProps.SEED_ENTITY_ID);
        }
        public static int GetCost(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(SeedProperties.COST);
        }
        public static bool IsTriggerActive(this SeedDefinition definition)
        {
            return definition.GetProperty<bool>(BuiltinSeedProps.TRIGGER_ACTIVE);
        }
        public static int GetTriggerCost(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(BuiltinSeedProps.TRIGGER_COST);
        }
    }
}
