using MVZ2.GameContent;
using PVZEngine.Level;
using Tools;

namespace MVZ2.Vanilla
{
    public static class VanillaCartExt
    {
        public static void SetTurnToMoneyTimer(this Entity cart, FrameTimer timer)
        {
            cart.SetProperty(VanillaCartProps.TURN_TO_MONEY_TIMER, timer);
        }
        public static FrameTimer GetTurnToMoneyTimer(this Entity cart)
        {
            return cart.GetProperty<FrameTimer>(VanillaCartProps.TURN_TO_MONEY_TIMER);
        }
    }
}
