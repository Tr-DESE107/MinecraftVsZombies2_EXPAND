using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    [PropertyRegistryRegion(PropertyRegions.recharge)]
    public static class EngineRechargeProps
    {
        public static readonly PropertyMeta START_MAX_RECHARGE = new PropertyMeta("startMaxRecharge");
        public static readonly PropertyMeta MAX_RECHARGE = new PropertyMeta("maxRecharge");
        public static readonly PropertyMeta QUALITY = new PropertyMeta("quality");
        public static readonly PropertyMeta NAME = new PropertyMeta("name");
        public static int GetStartMaxRecharge(this RechargeDefinition def)
        {
            return def.GetProperty<int>(EngineRechargeProps.START_MAX_RECHARGE);
        }
        public static int GetMaxRecharge(this RechargeDefinition def)
        {
            return def.GetProperty<int>(EngineRechargeProps.MAX_RECHARGE);
        }
        public static int GetQuality(this RechargeDefinition def)
        {
            return def.GetProperty<int>(EngineRechargeProps.QUALITY);
        }
        public static string GetName(this RechargeDefinition def)
        {
            return def.GetProperty<string>(EngineRechargeProps.NAME);
        }
    }
}
