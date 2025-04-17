using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Models;
using PVZEngine.Entities;
using UnityEngine;
namespace MVZ2.Vanilla.Entities
{
    public abstract class PickupBehaviour : VanillaEntityBehaviour, ICollectiblePickup
    {
        public PickupBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetSortingLayer(SortingLayers.pickups);
            entity.Timeout = entity.GetMaxTimeout();
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            if (!pickup.IsCollected())
            {
                LimitPosition(pickup);

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


                if (!pickup.IsImportantPickup() && pickup.Timeout >= 0)
                {
                    pickup.Timeout--;
                    if (pickup.Timeout <= 0)
                    {
                        pickup.Remove();
                    }
                }
            }
            else
            {
                pickup.AddPickupCollectedTime(1);
            }
        }
        private void LimitPosition(Entity entity)
        {
            Vector3 pos = entity.Position;
            pos.x = Mathf.Clamp(pos.x, VanillaLevelExt.GetPickupBorderX(false), VanillaLevelExt.GetPickupBorderX(true));
            entity.Position = pos;
        }
        public virtual void PostCollect(Entity pickup)
        {
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
