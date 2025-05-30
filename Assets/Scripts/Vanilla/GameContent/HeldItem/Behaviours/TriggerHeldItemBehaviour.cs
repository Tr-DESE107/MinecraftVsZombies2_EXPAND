using MVZ2.Vanilla.Entities;
using MVZ2Logic.HeldItems;
using PVZEngine.Entities;

namespace MVZ2.GameContent.HeldItems
{
    public class TriggerHeldItemBehaviour : ToEntityHeldItemBehaviour
    {
        public TriggerHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
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
            return entity.GetFaction() == entity.Level.Option.LeftFaction && entity.CanTrigger();
        }
        protected override void UseOnEntity(Entity entity)
        {
            entity.Trigger();
        }
    }
}
