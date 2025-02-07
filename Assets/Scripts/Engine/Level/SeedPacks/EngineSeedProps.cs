using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

namespace PVZEngine.Level
{
    [PropertyRegistryRegion]
    public static class EngineSeedProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta RECHARGE_ID = Get("rechargeId");
        public static readonly PropertyMeta COST = Get("cost");

        public static readonly PropertyMeta RECHARGE_SPEED = Get("rechargeSpeed");
        public static readonly PropertyMeta RECHARGE = Get("recharge");
        public static readonly PropertyMeta IS_START_RECHARGE = Get("isStartRecharge");
        public static readonly PropertyMeta DISABLED = Get("disabled");
        public static readonly PropertyMeta DISABLE_MESSAGE = Get("disableMessage");
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
            seed.SetProperty(RECHARGE, value);
        }
        public static NamespaceID GetRechargeID(this SeedPack seed)
        {
            return seed.GetProperty<NamespaceID>(RECHARGE_ID);
        }
        public static NamespaceID GetRechargeID(this SeedDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(RECHARGE_ID);
        }
        public static int GetCost(this SeedDefinition definition)
        {
            return definition.GetProperty<int>(COST);
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
            seed.SetProperty(IS_START_RECHARGE, value);
        }
        public static bool IsDisabled(this SeedPack seed)
        {
            return seed.GetProperty<bool>(DISABLED);
        }
        public static string GetDisableMessage(this SeedPack seed)
        {
            return seed.GetProperty<string>(DISABLE_MESSAGE);
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
