using MVZ2.GameContent.HeldItems;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.HeldItems
{
    [HeldItemDefinition(BuiltinHeldItemNames.none)]
    public class NoneHeldItemDefinition : HeldItemDefinition
    {
        public NoneHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new PickupHeldItemBehaviour(this));
            AddBehaviour(new TriggerCartHeldItemBehaviour(this));
        }
    }
}
