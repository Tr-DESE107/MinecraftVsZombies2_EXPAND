using PVZEngine.Entities;

namespace MVZ2.Vanilla.Contraptions
{
    public static class VanillaContraptionProps
    {
        public const string PLACE_ON_WATER = "placeOnWater";
        public const string PLACE_ON_LILY = "placeOnLily";
        public const string IS_FLOOR = "isFloor";
        public const string FRAGMENT_ID = "fragmentId";
        public const string TRIGGER_ACTIVE = "triggerActive";
        public const string INSTANT_TRIGGER = "instantTrigger";
        public static bool IsFloor(this Entity contraption)
        {
            return contraption.GetProperty<bool>(IS_FLOOR);
        }
        public static void SetTriggerActive(this Entity entity, bool value)
        {
            entity.SetProperty(TRIGGER_ACTIVE, value);
        }
        public static bool IsTriggerActive(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(TRIGGER_ACTIVE);
        }
        public static bool IsTriggerActive(this Entity entity)
        {
            return entity.GetProperty<bool>(TRIGGER_ACTIVE);
        }
        public static bool CanInstantTrigger(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(INSTANT_TRIGGER);
        }
    }
}
