using PVZEngine;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaCartProps
    {
        public const string CART_TRIGGER_SOUND = "cartTriggerSound";
        public const string TURN_TO_MONEY_TIMER = "turnToMoneyTimer";
        public static void SetTurnToMoneyTimer(this Entity cart, FrameTimer timer)
        {
            cart.SetProperty(TURN_TO_MONEY_TIMER, timer);
        }
        public static FrameTimer GetTurnToMoneyTimer(this Entity cart)
        {
            return cart.GetProperty<FrameTimer>(TURN_TO_MONEY_TIMER);
        }
        public static void SetCartTriggerSound(this Entity entity, NamespaceID value)
        {
            entity.SetProperty(CART_TRIGGER_SOUND, value);
        }
        public static NamespaceID GetCartTriggerSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(CART_TRIGGER_SOUND);
        }
    }
}
