using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Models;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.pickupCommon)]
    public class PickupCommonBehaviour : EntityBehaviourDefinition
    {
        public PickupCommonBehaviour(string nsp, string name) : base(nsp, name)
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

                var level = pickup.Level;
                if (!pickup.IsImportantPickup() && !level.IsHoldingEntity(pickup) && pickup.Timeout >= 0)
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
    }
}
