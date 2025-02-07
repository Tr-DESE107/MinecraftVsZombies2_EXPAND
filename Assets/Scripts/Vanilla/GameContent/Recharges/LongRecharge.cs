using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Recharges
{
    [RechargeDefinition(VanillaRechargeNames.longTime)]
    public class LongRecharge : RechargeDefinition
    {
        public LongRecharge(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineRechargeProps.START_MAX_RECHARGE, 600);
            SetProperty(EngineRechargeProps.MAX_RECHARGE, 900);
            SetProperty(EngineRechargeProps.QUALITY, 3);
            SetProperty(EngineRechargeProps.NAME, VanillaStrings.RECHARGE_LONG);
        }
    }
}
