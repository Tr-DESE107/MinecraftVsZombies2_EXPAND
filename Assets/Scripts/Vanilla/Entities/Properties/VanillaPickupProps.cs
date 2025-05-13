using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaPickupProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta COLLECTED_TIME = Get("collectedTime");
        public static readonly PropertyMeta IMPORTANT = Get("important");
        public static readonly PropertyMeta NO_AUTO_COLLECT = Get("noAutoCollect");
        public static readonly PropertyMeta DROP_SOUND = Get("dropSound");
        public static readonly PropertyMeta COLLECT_SOUND = Get("collectSound");
        public static readonly PropertyMeta ENERGY_VALUE = Get("energyValue");
        public static readonly PropertyMeta MONEY_VALUE = Get("moneyValue");
        public static readonly PropertyMeta REMOVE_ON_COLLECT = Get("removeOnCollect");
        public static bool IsImportantPickup(this Entity entity)
        {
            return entity.GetProperty<bool>(IMPORTANT);
        }
        public static int GetCollectedTime(this Entity entity)
        {
            return entity.GetProperty<int>(COLLECTED_TIME);
        }
        public static void SetCollectedTime(this Entity entity, int value)
        {
            entity.SetProperty(COLLECTED_TIME, value);
        }
        public static void AddPickupCollectedTime(this Entity entity, int value)
        {
            entity.SetCollectedTime(entity.GetCollectedTime() + value);
        }
        public static bool NoAutoCollect(this Entity pickup)
        {
            return pickup.GetProperty<bool>(NO_AUTO_COLLECT);
        }
        public static NamespaceID GetDropSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(DROP_SOUND);
        }
        public static void SetCollectSound(this Entity entity, NamespaceID value)
        {
            entity.SetProperty(COLLECT_SOUND, value);
        }
        public static NamespaceID GetCollectSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(COLLECT_SOUND);
        }
        public static int GetMoneyValue(this Entity entity)
        {
            return entity.GetProperty<int>(MONEY_VALUE);
        }
        public static int GetEnergyValue(this Entity entity)
        {
            return entity.GetProperty<int>(ENERGY_VALUE);
        }
        public static bool RemoveOnCollect(this Entity entity)
        {
            return entity.GetProperty<bool>(REMOVE_ON_COLLECT);
        }
        public static readonly PropertyMeta STRICT_COLLECT = Get("strictCollect");
        public static bool IsStrictCollect(this Entity pickup)
        {
            return pickup.GetProperty<bool>(STRICT_COLLECT);
        }
    }
}
