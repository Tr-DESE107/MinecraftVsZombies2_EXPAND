using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Modding;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Implements
{
    public class BlueprintImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, PostUseEntityBlueprintCallback);
        }
        private void PostUseEntityBlueprintCallback(VanillaLevelCallbacks.PostUseEntityBlueprintParams param, CallbackResult callbackResult)
        {
            var entity = param.entity;
            var seed = param.blueprint;
            var definition = param.definition;
            var heldData = param.heldData;
            if (entity == null)
                return;
            if (heldData.InstantTrigger && entity.CanTrigger())
            {
                entity.Trigger();
            }
            if (heldData.InstantEvoke && entity.CanEvoke() && entity.Level.GetStarshardCount() > 0 && !entity.Level.IsStarshardDisabled())
            {
                entity.Level.AddStarshardCount(-1);
                entity.Evoke();
                entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_USE_STARSHARD, new EntityCallbackParams(entity), entity.GetDefinitionID());
            }
            if (seed != null)
            {
                var drawnFromPool = seed.GetDrawnConveyorSeed();
                if (NamespaceID.IsValid(drawnFromPool))
                {
                    entity.AddTakenConveyorSeed(drawnFromPool);
                }
            }
        }
    }
}
