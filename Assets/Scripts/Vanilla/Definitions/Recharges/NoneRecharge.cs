using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Recharges
{
    [Definition(VanillaRechargeNames.none)]
    public class NoneRecharge : RechargeDefinition
    {
        public NoneRecharge(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineRechargeProps.START_MAX_RECHARGE, 0);
            SetProperty(EngineRechargeProps.MAX_RECHARGE, 0);
        }
    }
}
