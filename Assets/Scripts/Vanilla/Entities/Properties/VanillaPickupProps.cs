using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaPickupProps
    {
        public const string COLLECTED_TIME = "collectedTime";
        public const string IMPORTANT = "important";
        public const string NO_AUTO_COLLECT = "noAutoCollect";
        public const string DROP_SOUND = "dropSound";
        public const string COLLECT_SOUND = "collectSound";
        public const string MONEY_VALUE = "moneyValue";
        public const string REMOVE_ON_COLLECT = "removeOnCollect";
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
        public static bool CanAutoCollect(this Entity pickup)
        {
            return !pickup.GetProperty<bool>(NO_AUTO_COLLECT);
        }
        public static NamespaceID GetDropSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(DROP_SOUND);
        }
        public static NamespaceID GetCollectSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(COLLECT_SOUND);
        }
        public static int GetMoneyValue(this Entity entity)
        {
            return entity.GetProperty<int>(MONEY_VALUE);
        }
        public static bool RemoveOnCollect(this Entity entity)
        {
            return entity.GetProperty<bool>(REMOVE_ON_COLLECT);
        }
    }
}
