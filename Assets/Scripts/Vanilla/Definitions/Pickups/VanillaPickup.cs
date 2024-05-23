using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaPickup : VanillaEntity, ICollectiblePickup
    {
        public override void Update(Entity entity)
        {
            var pickup = entity.ToPickup();
            if (!pickup.IsCollected)
            {
                LimitPosition(entity);
                if (pickup.Game.IsAutoCollect() && pickup.CanAutoCollect() && !pickup.IsCollected && pickup.GetRelativeY() <= 0)
                {
                    pickup.Collect();
                }
            }
        }
        private void LimitPosition(Entity entity)
        {
            Vector3 pos = entity.Pos;
            pos.x = Mathf.Clamp(pos.x, MVZ2Game.GetPickupBorderX(false), MVZ2Game.GetPickupBorderX(true));
            entity.Pos = pos;
        }
        public virtual void PostCollect(Pickup pickup)
        {
        }
        public override int Type => EntityTypes.PICKUP;
    }
    public interface ICollectiblePickup
    {
        void PostCollect(Pickup pickup);
    }
}
