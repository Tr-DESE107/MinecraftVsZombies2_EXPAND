using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaPickup : EntityDefinition, ICollectiblePickup
    {
        public override void Update(Entity entity)
        {
            var pickup = entity.ToPickup();
            if (!pickup.IsCollected)
            {
                if (pickup.Game.IsAutoCollect() && pickup.CanAutoCollect() && !pickup.IsCollected && pickup.GetRelativeY() <= 0)
                {
                    pickup.Collect();
                }
            }
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
