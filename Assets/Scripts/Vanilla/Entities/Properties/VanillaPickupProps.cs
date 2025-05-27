using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class VanillaPickupProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<int> COLLECTED_TIME = Get<int>("collectedTime");
        public static readonly PropertyMeta<bool> IMPORTANT = Get<bool>("important");
        public static readonly PropertyMeta<bool> NO_AUTO_COLLECT = Get<bool>("noAutoCollect");
        public static readonly PropertyMeta<NamespaceID> DROP_SOUND = Get<NamespaceID>("dropSound");
        public static readonly PropertyMeta<NamespaceID> COLLECT_SOUND = Get<NamespaceID>("collectSound");
        public static readonly PropertyMeta<int> ENERGY_VALUE = Get<int>("energyValue");
        public static readonly PropertyMeta<int> MONEY_VALUE = Get<int>("moneyValue");
        public static readonly PropertyMeta<bool> REMOVE_ON_COLLECT = Get<bool>("removeOnCollect");
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
        public static readonly PropertyMeta<bool> STRICT_COLLECT = Get<bool>("strictCollect");
        public static bool IsStrictCollect(this Entity pickup)
        {
            return pickup.GetProperty<bool>(STRICT_COLLECT);
        }
    }
}
