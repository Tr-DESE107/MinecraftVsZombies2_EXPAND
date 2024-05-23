using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Seeds
{
    [Definition(RechargeNames.shortTime)]
    public class ShortRecharge : RechargeDefinition
    {
        public ShortRecharge() : base()
        {
            SetProperty(RechargeProperties.START_MAX_RECHARGE, 0);
            SetProperty(RechargeProperties.MAX_RECHARGE, 225);
        }
    }
}
