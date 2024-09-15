using MVZ2.GameContent;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaPickup : VanillaEntity, ICollectiblePickup
    {
        public VanillaPickup(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.GRAVITY, 1f);
            SetProperty(EntityProperties.FRICTION, 0.15f);
            SetProperty(EntityProperties.SIZE, new Vector3(32, 32, 32));
            SetProperty(BuiltinEntityProps.MAX_TIMEOUT, 300);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = entity.GetMaxTimeout();
        }
        public override void Update(Entity pickup)
        {
            if (!pickup.IsCollected())
            {
                LimitPosition(pickup);
                if (pickup.Level.IsAutoCollect() && pickup.CanAutoCollect() && pickup.GetRelativeY() <= 0)
                {
                    pickup.Collect();
                }
                if (!pickup.IsImportantPickup())
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
            Vector3 pos = entity.Pos;
            pos.x = Mathf.Clamp(pos.x, BuiltinLevel.GetPickupBorderX(false), BuiltinLevel.GetPickupBorderX(true));
            entity.Pos = pos;
        }
        public virtual void PostCollect(Entity pickup)
        {
        }
        public override int Type => EntityTypes.PICKUP;
    }
}
