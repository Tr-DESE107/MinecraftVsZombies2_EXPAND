using PVZEngine.Level;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class VanillaPickupProps
    {
        public const string DROP_SOUND = "dropSound";
        public const string COLLECT_SOUND = "collectSound";
        public const string MONEY_VALUE = "moneyValue";
        public const string REMOVE_ON_COLLECT = "removeOnCollect";
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
