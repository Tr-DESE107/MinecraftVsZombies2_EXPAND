using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.SeedPacks
{
    [PropertyRegistryRegion(PropertyRegions.seed)]
    public static class VanillaSeedProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<NamespaceID> SEED_ENTITY_ID = Get<NamespaceID>("seedEntityId");
        public static readonly PropertyMeta<NamespaceID> SEED_OPTION_ID = Get<NamespaceID>("seedOptionId");
        public static readonly PropertyMeta<int> SEED_TYPE = Get<int>("seedType");
        public static readonly PropertyMeta<bool> TWINKLING = Get<bool>("twinkling");
        public static readonly PropertyMeta<bool> TRIGGER_ACTIVE = Get<bool>("triggerActive");
        public static readonly PropertyMeta<bool> CAN_INSTANT_EVOKE = Get<bool>("canInstantEvoke");
        public static readonly PropertyMeta<bool> CAN_INSTANT_TRIGGER = Get<bool>("canInstantTrigger");
        public static readonly PropertyMeta<NamespaceID> DRAWN_CONVEYOR_SEED = Get<NamespaceID>("drawnConveyorSeed");
        public static readonly PropertyMeta<bool> UPGRADE_BLUEPRINT = Get<bool>("upgradeBlueprint");
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
        public static bool WillInstantEvoke(this SeedDefinition definition, LevelEngine level)
        {
            if (definition == null)
                return false;
            if (!definition.CanInstantEvoke())
                return false;
            if (level.IsDay())
            {
                var entityID = definition.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                if (entityDef.IsNocturnal())
                    return false;
            }
            return true;
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
        public static readonly PropertyMeta<bool> COMMAND_BLOCK = Get<bool>("command_block");
        public static bool IsCommandBlock(this SeedPack seed)
        {
            return seed.GetProperty<bool>(COMMAND_BLOCK);
        }
        public static void SetCommandBlock(this SeedPack seed, bool value)
        {
            seed.SetProperty(COMMAND_BLOCK, value);
        }
    }
}
