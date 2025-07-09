using MVZ2.GameContent.Models;
using MVZ2.GameContent.Pickups;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.starshard)]
    public class StarshardHeldItemBehaviour : ToEntityHeldItemBehaviour, IHeldTwinkleEntityBehaviour
    {
        public StarshardHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void GetModelID(LevelEngine level, IHeldItemData data, CallbackResult result)
        {
            var modelID = VanillaModelID.GetStarshardHeldItem(level.AreaDefinition.GetID());
            if (Global.Game.GetModelMeta(modelID) == null)
            {
                modelID = VanillaModelID.defaultStartShardHeldItem;
            }
            result.SetFinalValue(modelID);
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
        public bool ShouldMakeEntityTwinkle(Entity entity, IHeldItemData data)
        {
            var seedDefinition = BlueprintPickup.GetSeedDefinition(entity);
            return seedDefinition.WillInstantEvoke(entity.Level);
        }
    }
}
