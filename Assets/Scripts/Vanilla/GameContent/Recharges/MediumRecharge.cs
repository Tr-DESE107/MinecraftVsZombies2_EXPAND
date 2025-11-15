using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Recharges
{
    [RechargeDefinition(VanillaRechargeNames.mediumTime)]
    public class MediumRecharge : RechargeDefinition
    {
        public MediumRecharge(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineRechargeProps.START_MAX_RECHARGE, 300);
            SetProperty(EngineRechargeProps.MAX_RECHARGE, 450);
            SetProperty(EngineRechargeProps.QUALITY, 2);
            SetProperty(EngineRechargeProps.NAME, VanillaStrings.RECHARGE_MEDIUM);
        }
    }
}
