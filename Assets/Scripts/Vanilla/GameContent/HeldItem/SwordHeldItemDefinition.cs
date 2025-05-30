using MVZ2Logic;
using MVZ2Logic.HeldItems;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.sword)]
    public class SwordHeldItemDefinition : HeldItemDefinition
    {
        public SwordHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(VanillaHeldItemBehaviourID.pickup);
            AddBehaviour(VanillaHeldItemBehaviourID.triggerCart);
            AddBehaviour(VanillaHeldItemBehaviourID.selectBlueprint);
            AddBehaviour(VanillaHeldItemBehaviourID.sword);
        }
    }
}
