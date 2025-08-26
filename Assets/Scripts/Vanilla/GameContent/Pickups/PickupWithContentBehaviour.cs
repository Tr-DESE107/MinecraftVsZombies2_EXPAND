using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.pickupWithContent)]
    public class PickupWithContentBehaviour : EntityBehaviourDefinition
    {
        public PickupWithContentBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            pickup.SetModelProperty("ContentID", pickup.GetPickupContentID());
        }
    }
}
