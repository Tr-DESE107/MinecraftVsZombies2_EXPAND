using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public static class VanillaPickupExt
    {
        public static NamespaceID GetDropSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(PickupProps.DROP_SOUND);
        }
        public static NamespaceID GetCollectSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(PickupProps.COLLECT_SOUND);
        }
        public static int GetMoneyValue(this Entity entity)
        {
            return entity.GetProperty<int>(PickupProps.MONEY_VALUE);
        }
        public static bool RemoveOnCollect(this Entity entity)
        {
            return entity.GetProperty<bool>(PickupProps.REMOVE_ON_COLLECT);
        }
    }
}
