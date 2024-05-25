using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Recharges
{
    [Definition(RechargeNames.none)]
    public class NoneRecharge : RechargeDefinition
    {
        public NoneRecharge() : base()
        {
            SetProperty(RechargeProperties.START_MAX_RECHARGE, 0);
            SetProperty(RechargeProperties.MAX_RECHARGE, 0);
        }
    }
}
