using MVZ2.GameContent.HeldItem;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.trigger)]
    public class TriggerHeldItemDefinition : HeldItemDefinition
    {
        public TriggerHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new PickupHeldItemBehaviour(this));
            AddBehaviour(new TriggerCartHeldItemBehaviour(this));
            AddBehaviour(new TriggerHeldItemBehaviour(this));
        }

        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            return VanillaModelID.triggerHeldItem;
        }
    }
}
