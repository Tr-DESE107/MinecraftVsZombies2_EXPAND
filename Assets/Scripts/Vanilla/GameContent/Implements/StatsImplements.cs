using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Implements
{
    public class StatsImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, PostUseEntityBlueprintCallback);
            mod.AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_DESTROY, PostContraptionDestroyCallback);
            mod.AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_EVOKE, PostContraptionEvokeCallback);

            mod.AddTrigger(LevelCallbacks.POST_ENEMY_SPAWNED, PostEnemySpawnedCallback);
            mod.AddTrigger(VanillaLevelCallbacks.POST_ENEMY_NEUTRALIZE, PostEnemyNeutralizeCallback);
            mod.AddTrigger(LevelCallbacks.POST_GAME_OVER, PostGameOverCallback);
        }
        private void PostUseEntityBlueprintCallback(VanillaLevelCallbacks.PostUseEntityBlueprintParams param, CallbackResult callbackResult)
        {
            var entity = param.entity;
            var seed = param.blueprint;
            var definition = param.definition;
            var heldData = param.heldData;
            if (entity.Type != EntityTypes.PLANT)
                return;
            Global.AddSaveStat(VanillaStats.CATEGORY_CONTRAPTION_PLACE, entity.GetDefinitionID(), 1);
        }
        private void PostContraptionDestroyCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            Global.AddSaveStat(VanillaStats.CATEGORY_CONTRAPTION_DESTROY, entity.GetDefinitionID(), 1);
        }
        private void PostContraptionEvokeCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            Global.AddSaveStat(VanillaStats.CATEGORY_CONTRAPTION_EVOKE, entity.GetDefinitionID(), 1);
        }
        private void PostEnemySpawnedCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            Global.AddSaveStat(VanillaStats.CATEGORY_ENEMY_SPAWN, entity.GetDefinitionID(), 1);
        }
        private void PostEnemyNeutralizeCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            Global.AddSaveStat(VanillaStats.CATEGORY_ENEMY_NEUTRALIZE, entity.GetDefinitionID(), 1);
        }
        private void PostGameOverCallback(LevelCallbacks.PostGameOverParams param, CallbackResult result)
        {
            var killer = param.killer;
            if (killer == null)
                return;
            Global.AddSaveStat(VanillaStats.CATEGORY_ENEMY_GAME_OVER, killer.GetDefinitionID(), 1);
        }
    }
}
