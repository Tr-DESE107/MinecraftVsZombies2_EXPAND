using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Recharges
{
    [Definition(VanillaRechargeNames.veryLongTime)]
    public class VeryLongRecharge : RechargeDefinition
    {
        public VeryLongRecharge(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineRechargeProps.START_MAX_RECHARGE, 1050);
            SetProperty(EngineRechargeProps.MAX_RECHARGE, 1500);
            SetProperty(EngineRechargeProps.QUALITY, 6);
            SetProperty(EngineRechargeProps.NAME, VanillaStrings.RECHARGE_VERY_LONG);
        }
    }
}
