using MVZ2.GameContent.HeldItem;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.starshard)]
    public class StarshardHeldItemDefinition : HeldItemDefinition
    {
        public StarshardHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new PickupHeldItemBehaviour(this));
            AddBehaviour(new TriggerCartHeldItemBehaviour(this));
            AddBehaviour(new StarshardHeldItemBehaviour(this));
        }
        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            var modelID = VanillaModelID.GetStarshardHeldItem(level.AreaDefinition.GetID());
            if (Global.Game.GetModelMeta(modelID) == null)
            {
                return VanillaModelID.defaultStartShardHeldItem;
            }
            return modelID;
        }
    }
}
