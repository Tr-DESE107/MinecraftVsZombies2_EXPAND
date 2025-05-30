using MVZ2Logic;
using MVZ2Logic.HeldItems;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.forcePad)]
    public class ForcePadHeldItemDefinition : EntityHeldItemDefinition
    {
        public ForcePadHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(VanillaHeldItemBehaviourID.forcePad);
        }
    }
}
