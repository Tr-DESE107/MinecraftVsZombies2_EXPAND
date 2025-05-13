using PVZEngine;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaCartProps
    {
        public static readonly PropertyMeta CART_TRIGGER_SOUND = new PropertyMeta("cartTriggerSound");
        public static readonly PropertyMeta TURN_TO_MONEY_TIMER = new PropertyMeta("turnToMoneyTimer");
        public static void SetTurnToMoneyTimer(this Entity cart, FrameTimer timer) => cart.SetProperty(TURN_TO_MONEY_TIMER, timer);
        public static FrameTimer GetTurnToMoneyTimer(this Entity cart) => cart.GetProperty<FrameTimer>(TURN_TO_MONEY_TIMER);
        public static void SetCartTriggerSound(this Entity entity, NamespaceID value) => entity.SetProperty(CART_TRIGGER_SOUND, value);
        public static NamespaceID GetCartTriggerSound(this Entity entity) => entity.GetProperty<NamespaceID>(CART_TRIGGER_SOUND);

    }
}
