using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Recharges
{
    [Definition(RechargeNames.longTime)]
    public class LongRecharge : RechargeDefinition
    {
        public LongRecharge(string nsp, string name) : base(nsp, name)
        {
            SetProperty(RechargeProperties.START_MAX_RECHARGE, 600);
            SetProperty(RechargeProperties.MAX_RECHARGE, 900);
        }
    }
}
