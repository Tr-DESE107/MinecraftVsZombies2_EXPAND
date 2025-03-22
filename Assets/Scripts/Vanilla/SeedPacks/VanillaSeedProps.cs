using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.SeedPacks
{
    [PropertyRegistryRegion]
    public static class VanillaSeedProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta SEED_ENTITY_ID = Get("seedEntityId");
        public static readonly PropertyMeta SEED_OPTION_ID = Get("seedOptionId");
        public static readonly PropertyMeta SEED_TYPE = Get("seedType");
        public static readonly PropertyMeta TWINKLING = Get("twinkling");
        public static readonly PropertyMeta TRIGGER_ACTIVE = Get("triggerActive");
        public static readonly PropertyMeta CAN_INSTANT_EVOKE = Get("canInstantEvoke");
        public static readonly PropertyMeta CAN_INSTANT_TRIGGER = Get("canInstantTrigger");
        public static readonly PropertyMeta DRAWN_CONVEYOR_SEED = Get("drawnConveyorSeed");
        public static readonly PropertyMeta UPGRADE_BLUEPRINT = Get("upgradeBlueprint");
        public static bool IsUpgradeBlueprint(this SeedPack seed)
        {
            return seed.GetProperty<bool>(UPGRADE_BLUEPRINT);
        }
        public static bool IsUpgradeBlueprint(this SeedDefinition definition)
        {
            return definition.GetProperty<bool>(UPGRADE_BLUEPRINT);
        }
        public static int GetSeedType(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(SEED_TYPE);
        }
        public static NamespaceID GetSeedEntityID(this SeedDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(SEED_ENTITY_ID);
        }
        public static NamespaceID GetSeedOptionID(this SeedDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(SEED_OPTION_ID);
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
        public static bool CanInstantEvoke(this SeedDefinition definition)
        {
            return definition.GetProperty<bool>(CAN_INSTANT_EVOKE);
        }
        public static bool IsTwinkling(this SeedPack seed)
        {
            return seed.GetProperty<bool>(TWINKLING);
        }
        public static void SetTwinkling(this SeedPack seed, bool value)
        {
            seed.SetProperty(TWINKLING, value);
        }
        public static NamespaceID GetDrawnConveyorSeed(this SeedPack seed)
        {
            return seed.GetProperty<NamespaceID>(DRAWN_CONVEYOR_SEED);
        }
        public static void SetDrawnConveyorSeed(this SeedPack seed, NamespaceID value)
        {
            seed.SetProperty(DRAWN_CONVEYOR_SEED, value);
        }
    }
}
