using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.pickupLimitPosition)]
    public class PickupLimitPositionBehaviour : EntityBehaviourDefinition
    {
        public PickupLimitPositionBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            if (!pickup.IsCollected())
            {
                Vector3 pos = pickup.Position;
                pos.x = Mathf.Clamp(pos.x, VanillaLevelExt.GetPickupBorderX(false), VanillaLevelExt.GetPickupBorderX(true));
                pickup.Position = pos;
            }
        }
    }
}
