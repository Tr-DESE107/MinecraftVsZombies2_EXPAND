using MVZ2.GameContent;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public static class BuiltinPickup
    {
        public static bool IsCollected(this Entity entity)
        {
            return entity.State == EntityStates.COLLECTED;
        }
        public static void Collect(this Entity pickup)
        {
            pickup.State = EntityStates.COLLECTED;
            if (pickup.Definition is ICollectiblePickup collectible)
                collectible.PostCollect(pickup);
        }
        public static bool IsImportantPickup(this Entity entity)
        {
            return entity.GetProperty<bool>(BuiltinPickupProps.IMPORTANT);
        }
        public static int GetCollectedTime(this Entity entity)
        {
            return entity.GetProperty<int>(BuiltinPickupProps.COLLECTED_TIME);
        }
        public static void SetCollectedTime(this Entity entity, int value)
        {
            entity.SetProperty(BuiltinPickupProps.COLLECTED_TIME, value);
        }
        public static void AddPickupCollectedTime(this Entity entity, int value)
        {
            entity.SetCollectedTime(entity.GetCollectedTime() + value);
        }
        public static bool CanAutoCollect(this Entity pickup)
        {
            return !pickup.GetProperty<bool>(BuiltinPickupProps.NO_AUTO_COLLECT);
        }
    }
}
