using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.HeldItems;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.HeldItems
{
    public class StarshardHeldItemBehaviour : ToEntityHeldItemBehaviour
    {
        public StarshardHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        protected override bool CanUseOnEntity(Entity entity)
        {
            if (!entity.ExistsAndAlive())
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
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_USE_STARSHARD, new EntityCallbackParams(entity), entity.GetDefinitionID());
        }
    }
}
