using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent
{
    public static class VanillaCartProps
    {
        public const string TURN_TO_MONEY_TIMER = "turnToMoneyTimer";
        public static void SetTurnToMoneyTimer(this Entity cart, FrameTimer timer)
        {
            cart.SetProperty(TURN_TO_MONEY_TIMER, timer);
        }
        public static FrameTimer GetTurnToMoneyTimer(this Entity cart)
        {
            return cart.GetProperty<FrameTimer>(TURN_TO_MONEY_TIMER);
        }
    }
}
