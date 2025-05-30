using MVZ2.GameContent.HeldItems;
using MVZ2Logic.HeldItems;

namespace MVZ2.Vanilla.HeldItems
{
    public abstract class BlueprintHeldItemDefinition : HeldItemDefinition
    {
        public BlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(VanillaHeldItemBehaviourID.rightMouseCancel);
            AddBehaviour(VanillaHeldItemBehaviourID.pickup);
            AddBehaviour(VanillaHeldItemBehaviourID.triggerCart);
            AddBehaviour(VanillaHeldItemBehaviourID.selectBlueprint);
        }
    }
}
