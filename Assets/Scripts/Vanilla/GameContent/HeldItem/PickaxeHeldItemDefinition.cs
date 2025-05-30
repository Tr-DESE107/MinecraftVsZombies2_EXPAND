using MVZ2Logic;
using MVZ2Logic.HeldItems;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.pickaxe)]
    public class PickaxeHeldItemDefinition : HeldItemDefinition
    {
        public PickaxeHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(VanillaHeldItemBehaviourID.rightMouseCancel);
            AddBehaviour(VanillaHeldItemBehaviourID.pickup);
            AddBehaviour(VanillaHeldItemBehaviourID.triggerCart);
            AddBehaviour(VanillaHeldItemBehaviourID.selectBlueprint);
            AddBehaviour(VanillaHeldItemBehaviourID.pickaxe);
        }
    }
}
