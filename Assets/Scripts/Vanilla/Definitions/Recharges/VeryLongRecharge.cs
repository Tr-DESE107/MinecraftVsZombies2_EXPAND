using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Recharges
{
    [Definition(RechargeNames.veryLongTime)]
    public class VeryLongRecharge : RechargeDefinition
    {
        public VeryLongRecharge(string nsp, string name) : base(nsp, name)
        {
            SetProperty(RechargeProperties.START_MAX_RECHARGE, 1050);
            SetProperty(RechargeProperties.MAX_RECHARGE, 1500);
        }
    }
}
