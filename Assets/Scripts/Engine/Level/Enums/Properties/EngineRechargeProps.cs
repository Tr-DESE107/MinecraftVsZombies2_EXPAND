using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public static class EngineRechargeProps
    {
        public const string START_MAX_RECHARGE = "startMaxRecharge";
        public const string MAX_RECHARGE = "maxRecharge";
        public static int GetStartMaxRecharge(this RechargeDefinition def)
        {
            return def.GetProperty<int>(EngineRechargeProps.START_MAX_RECHARGE);
        }
        public static int GetMaxRecharge(this RechargeDefinition def)
        {
            return def.GetProperty<int>(EngineRechargeProps.MAX_RECHARGE);
        }
    }
}
