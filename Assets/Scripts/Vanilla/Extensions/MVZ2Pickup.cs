using MVZ2.GameContent;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Pickup
    {
        public static bool CanAutoCollect(this Pickup pickup)
        {
            return !pickup.GetProperty<bool>(PickupProperties.NO_AUTO_COLLECT);
        }
        public static void Collect(this Pickup pickup)
        {
            pickup.IsCollected = true;
            pickup.Velocity = Vector3.zero;
            if (pickup.Definition is ICollectiblePickup collectible)
                collectible.PostCollect(pickup);
        }
    }
}
