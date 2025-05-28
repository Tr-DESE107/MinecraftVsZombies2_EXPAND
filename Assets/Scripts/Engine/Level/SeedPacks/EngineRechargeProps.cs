﻿using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    [PropertyRegistryRegion(PropertyRegions.recharge)]
    public static class EngineRechargeProps
    {
        public static readonly PropertyMeta<int> START_MAX_RECHARGE = new PropertyMeta<int>("startMaxRecharge");
        public static int GetStartMaxRecharge(this RechargeDefinition def)
        {
            return def.GetProperty<int>(EngineRechargeProps.START_MAX_RECHARGE);
        }
        public static readonly PropertyMeta<int> MAX_RECHARGE = new PropertyMeta<int>("maxRecharge");
        public static int GetMaxRecharge(this RechargeDefinition def)
        {
            return def.GetProperty<int>(EngineRechargeProps.MAX_RECHARGE);
        }
        public static readonly PropertyMeta<int> QUALITY = new PropertyMeta<int>("quality");
        public static int GetQuality(this RechargeDefinition def)
        {
            return def.GetProperty<int>(EngineRechargeProps.QUALITY);
        }
        public static readonly PropertyMeta<string> NAME = new PropertyMeta<string>("name");
        public static string GetName(this RechargeDefinition def)
        {
            return def.GetProperty<string>(EngineRechargeProps.NAME);
        }
    }
}
