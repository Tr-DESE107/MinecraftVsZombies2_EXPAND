using PVZEngine.Entities;

namespace MVZ2.Vanilla.Contraptions
{
    public static class VanillaContraptionProps
    {
        #region 可放置性
        public const string PLACE_ON_LAND = "placeOnLand";
        public const string PLACE_ON_WATER = "placeOnWater";
        public const string PLACE_ON_LILY = "placeOnLilypad";
        public const string PLACE_ON_PLANE = "placeOnPlane";
        public static bool CanPlaceOnWater(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(PLACE_ON_WATER);
        }
        public static bool CanPlaceOnWater(this Entity entity)
        {
            return entity.GetProperty<bool>(PLACE_ON_WATER);
        }
        public static bool CanPlaceOnLilypad(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(PLACE_ON_LILY);
        }
        public static bool CanPlaceOnLilypad(this Entity entity)
        {
            return entity.GetProperty<bool>(PLACE_ON_LILY);
        }
        public static bool CanPlaceOnLand(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(PLACE_ON_LAND);
        }
        public static bool CanPlaceOnPlane(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(PLACE_ON_PLANE);
        }
        #endregion

        #region 夜用
        public const string NOCTURNAL = "nocturnal";
        public static bool IsNocturnal(this EntityDefinition definition)
        {
            return definition.GetProperty<bool>(NOCTURNAL);
        }
        public static bool IsNocturnal(this Entity entity)
        {
            return entity.GetProperty<bool>(NOCTURNAL);
        }
        #endregion

        public const string IS_FLOOR = "isFloor";
        public const string CAN_SHOCK = "canShock";
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
        public static bool CanShock(this Entity entity)
        {
            return entity.GetProperty<bool>(CAN_SHOCK);
        }
    }
}
