using MVZ2.GameContent.Models;
using MVZ2.GameContent.Pickups;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.trigger)]
    public class TriggerHeldItemBehaviour : ToEntityHeldItemBehaviour, IHeldTwinkleEntityBehaviour
    {
        public TriggerHeldItemBehaviour(string nsp, string name) : base(nsp, name)
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

        public override void GetModelID(LevelEngine level, IHeldItemData data, CallbackResult result)
        {
            result.SetFinalValue(VanillaModelID.triggerHeldItem);
        }

        public bool ShouldMakeEntityTwinkle(Entity entity, IHeldItemData data)
        {
            if (entity.CanTrigger())
                return true;
            var seedDefinition = BlueprintPickup.GetSeedDefinition(entity);
            return seedDefinition != null && seedDefinition.IsTriggerActive() && seedDefinition.CanInstantTrigger();
        }
    }
}
