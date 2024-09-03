using MVZ2.Vanilla;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Recharges
{
    [Definition(RechargeNames.shortTime)]
    public class ShortRecharge : RechargeDefinition
    {
        public ShortRecharge(string nsp, string name) : base(nsp, name)
        {
            SetProperty(RechargeProperties.START_MAX_RECHARGE, 0);
            SetProperty(RechargeProperties.MAX_RECHARGE, 225);
        }
    }
}
