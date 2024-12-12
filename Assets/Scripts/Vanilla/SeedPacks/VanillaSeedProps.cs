using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.SeedPacks
{
    public static class VanillaSeedProps
    {
        public const string SEED_ENTITY_ID = "seedEntityId";
        public const string SEED_TYPE = "seedType";
        public const string TWINKLING = "twinkling";
        public const string TRIGGER_ACTIVE = "triggerActive";
        public const string CAN_INSTANT_TRIGGER = "canInstantTrigger";
        public static int GetSeedType(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(SEED_TYPE);
        }
        public static NamespaceID GetSeedEntityID(this SeedDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(SEED_ENTITY_ID);
        }
        public static bool IsTriggerActive(this SeedDefinition definition)
        {
            return definition.GetProperty<bool>(TRIGGER_ACTIVE);
        }
        public static bool IsTriggerActive(this SeedPack seedPack)
        {
            return seedPack.GetProperty<bool>(TRIGGER_ACTIVE);
        }
        public static bool CanInstantTrigger(this SeedDefinition definition)
        {
            return definition.GetProperty<bool>(CAN_INSTANT_TRIGGER);
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
