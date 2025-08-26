﻿using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

namespace MVZ2Logic.SeedPacks
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


        public static readonly PropertyMeta<SpriteReference> MOBILE_ICON = Get<SpriteReference>("mobile_icon");
        public static SpriteReference GetMobileIcon(this SeedDefinition seed)
        {
            return seed.GetProperty<SpriteReference>(MOBILE_ICON);
        }
        public static SpriteReference GetMobileIcon(this SeedPack seed)
        {
            return seed.GetProperty<SpriteReference>(MOBILE_ICON);
        }
        public static void SetMobileIcon(this SeedDefinition seed, SpriteReference value)
        {
            seed.SetProperty(MOBILE_ICON, value);
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
