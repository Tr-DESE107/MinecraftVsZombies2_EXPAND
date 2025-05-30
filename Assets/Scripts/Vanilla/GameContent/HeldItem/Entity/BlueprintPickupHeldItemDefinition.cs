using MVZ2Logic;
using MVZ2Logic.HeldItems;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.blueprintPickup)]
    public class BlueprintPickupHeldItemDefinition : EntityHeldItemDefinition
    {
        public BlueprintPickupHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(VanillaHeldItemBehaviourID.rightMouseCancel);
            AddBehaviour(VanillaHeldItemBehaviourID.pickup);
            AddBehaviour(VanillaHeldItemBehaviourID.triggerCart);
            AddBehaviour(VanillaHeldItemBehaviourID.selectBlueprint);
            AddBehaviour(VanillaHeldItemBehaviourID.blueprintPickup);
        }
    }
}
