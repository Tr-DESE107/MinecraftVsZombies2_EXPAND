using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Games;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaPickup : VanillaEntity, ICollectiblePickup
    {
        public VanillaPickup(string nsp, string name) : base(nsp, name)
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
            if (!pickup.IsCollected())
            {
                LimitPosition(pickup);
                if (pickup.Level.IsAutoCollect() && pickup.CanAutoCollect() && pickup.GetRelativeY() <= 0)
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
            pos.x = Mathf.Clamp(pos.x, BuiltinLevel.GetPickupBorderX(false), BuiltinLevel.GetPickupBorderX(true));
            entity.Position = pos;
        }
        public virtual bool? PreCollect(Entity pickup)
        {
            return null;
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
        public override int Type => EntityTypes.PICKUP;
    }
}
