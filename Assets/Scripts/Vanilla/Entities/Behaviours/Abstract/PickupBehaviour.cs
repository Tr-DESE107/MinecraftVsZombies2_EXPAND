using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Models;
using PVZEngine.Entities;
namespace MVZ2.Vanilla.Entities
{
    public abstract class PickupBehaviour : EntityBehaviourDefinition, ICollectiblePickup
    {
        public PickupBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = entity.GetMaxTimeout();
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            if (!pickup.IsCollected() && !pickup.NoCollect())
            {
                var level = pickup.Level;
                var willAutoCollect = false;
                if (level.IsAutoCollectAll())
                {
                    willAutoCollect = true;
                }
                else if (pickup.GetEnergyValue() > 0 && level.IsAutoCollectEnergy())
                {
                    willAutoCollect = true;
                }
                else if (pickup.GetMoneyValue() > 0 && level.IsAutoCollectMoney())
                {
                    willAutoCollect = true;
                }
                else if (pickup.IsEntityOf(VanillaPickupID.starshard) && level.IsAutoCollectStarshard())
                {
                    willAutoCollect = true;
                }
                if (willAutoCollect && !pickup.NoAutoCollect() && CanAutoCollect(pickup) && CanCollect(pickup))
                {
                    pickup.Collect();
                }
            }
        }
        public virtual void PostCollect(Entity pickup)
        {
            pickup.State = VanillaEntityStates.PICKUP_COLLECTED;
            if (pickup.RemoveOnCollect())
            {
                pickup.Remove();
            }
            pickup.SetSortingLayer(SortingLayers.frontUI);
            pickup.SetSortingOrder(9999);
        }
        public virtual bool CanCollect(Entity pickup)
        {
            return true;
        }
        public virtual bool CanAutoCollect(Entity pickup)
        {
            return pickup.GetRelativeY() <= 0;
        }
    }
}
