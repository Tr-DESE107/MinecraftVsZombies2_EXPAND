using MVZ2Logic;
using MVZ2Logic.HeldItems;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.trigger)]
    public class TriggerHeldItemDefinition : HeldItemDefinition
    {
        public TriggerHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(VanillaHeldItemBehaviourID.rightMouseCancel);
            AddBehaviour(VanillaHeldItemBehaviourID.pickup);
            AddBehaviour(VanillaHeldItemBehaviourID.triggerCart);
            AddBehaviour(VanillaHeldItemBehaviourID.selectBlueprint);
            AddBehaviour(VanillaHeldItemBehaviourID.trigger);
        }
    }
}
