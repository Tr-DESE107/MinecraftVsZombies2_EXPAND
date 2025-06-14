using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

namespace PVZEngine.Level
{
    [PropertyRegistryRegion(PropertyRegions.seed)]
    public static class EngineSeedProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<NamespaceID> RECHARGE_ID = Get<NamespaceID>("rechargeId");
        public static readonly PropertyMeta<float> COST = Get<float>("cost");

        public static readonly PropertyMeta<float> RECHARGE_SPEED = Get<float>("rechargeSpeed");
        public static readonly PropertyMeta<float> RECHARGE = Get<float>("recharge");
        public static readonly PropertyMeta<bool> IS_START_RECHARGE = Get<bool>("isStartRecharge");
        public static readonly PropertyMeta<NamespaceID> DISABLE_ID = Get<NamespaceID>("disableID");
        public static float GetRechargeSpeed(this SeedPack seed)
        {
            return seed.GetProperty<float>(RECHARGE_SPEED);
        }
        public static float GetRecharge(this SeedPack seed)
        {
            return seed.GetProperty<float>(RECHARGE);
        }
        public static void SetRecharge(this SeedPack seed, float value)
        {
            seed.SetProperty<float>(RECHARGE, value);
        }
        public static NamespaceID GetRechargeID(this SeedPack seed)
        {
            return seed.GetProperty<NamespaceID>(RECHARGE_ID);
        }
        public static NamespaceID GetRechargeID(this SeedDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(RECHARGE_ID);
        }
        public static float GetCost(this SeedDefinition definition)
        {
            return definition.GetProperty<float>(COST);
        }
        public static bool IsStartRecharge(this SeedPack seed)
        {
            return seed.GetProperty<bool>(IS_START_RECHARGE);
        }
        /// <summary>
        /// 将卡牌的重装载时间设置为初始或已被使用。
        /// </summary>
        public static void SetStartRecharge(this SeedPack seed, bool value)
        {
            seed.SetProperty<bool>(IS_START_RECHARGE, value);
        }
        public static bool IsDisabled(this SeedPack seed)
        {
            return NamespaceID.IsValid(seed.GetDisableID());
        }
        public static NamespaceID GetDisableID(this SeedPack seed)
        {
            return seed.GetProperty<NamespaceID>(DISABLE_ID);
        }
        public static void FullRecharge(this SeedPack seed)
        {
            seed.SetRecharge(seed.GetMaxRecharge());
        }
        public static bool IsCharged(this SeedPack seed)
        {
            return seed.GetRecharge() >= seed.GetMaxRecharge();
        }
        public static void ResetRecharge(this SeedPack seed)
        {
            seed.SetRecharge(0);
        }
        public static int GetMaxRecharge(this SeedPack seed)
        {
            return seed.IsStartRecharge() ? seed.GetStartMaxRecharge() : seed.GetUsedMaxRecharge();
        }
    }
}
