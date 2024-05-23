using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Seeds
{
    [Definition(RechargeNames.veryLongTime)]
    public class VeryLongRecharge : RechargeDefinition
    {
        public VeryLongRecharge() : base()
        {
            SetProperty(RechargeProperties.START_MAX_RECHARGE, 1050);
            SetProperty(RechargeProperties.MAX_RECHARGE, 1500);
        }
    }
}
