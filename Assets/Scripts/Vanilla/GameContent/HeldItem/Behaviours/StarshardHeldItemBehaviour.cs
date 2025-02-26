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
    public class StarshardHeldItemBehaviour : ToEntityHeldItemBehaviour
    {
        public StarshardHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        protected override bool CanUseOnEntity(Entity entity)
        {
            if (entity == null)
                return false;
            if (entity.Type != EntityTypes.PLANT)
                return false;
            if (entity.NoHeldTarget())
                return false;
            return entity.GetFaction() == entity.Level.Option.LeftFaction && entity.CanEvoke();
        }
        protected override void UseOnEntity(Entity entity)
        {
            entity.Level.AddStarshardCount(-1);
            entity.Evoke();
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_USE_STARSHARD, entity.GetDefinitionID(), c => c(entity));
        }
    }
}
