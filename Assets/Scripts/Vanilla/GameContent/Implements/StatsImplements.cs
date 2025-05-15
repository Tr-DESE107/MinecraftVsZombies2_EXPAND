using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using static UnityEngine.EventSystems.EventTrigger;

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
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
            mod.AddTrigger(LevelCallbacks.POST_GAME_OVER, PostGameOverCallback);
        }
        private void PostUseEntityBlueprintCallback(VanillaLevelCallbacks.PostUseEntityBlueprintParams param, CallbackResult callbackResult)
        {
            var entity = param.entity;
            var seed = param.blueprint;
            var definition = param.definition;
            var heldData = param.heldData;
            var level = entity.Level;
            if (entity.Level.IsIZombie())
            {
                if (entity.Type == EntityTypes.ENEMY)
                {
                    Global.AddSaveStat(VanillaStats.CATEGORY_IZ_ENEMY_PLACE, entity.GetDefinitionID(), 1);
                }
            }
            else
            {
                if (entity.Type == EntityTypes.PLANT)
                {
                    Global.AddSaveStat(VanillaStats.CATEGORY_CONTRAPTION_PLACE, entity.GetDefinitionID(), 1);
                }
            }
        }
        private void PostContraptionDestroyCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            if (entity.Level.IsIZombie())
            {
                Global.AddSaveStat(VanillaStats.CATEGORY_IZ_CONTRAPTION_DESTROY, entity.GetDefinitionID(), 1);
            }
            else
            {
                Global.AddSaveStat(VanillaStats.CATEGORY_CONTRAPTION_DESTROY, entity.GetDefinitionID(), 1);
            }
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
            if (!entity.Level.IsIZombie())
            {
                Global.AddSaveStat(VanillaStats.CATEGORY_ENEMY_NEUTRALIZE, entity.GetDefinitionID(), 1);
            }
        }
        private void PostEnemyDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            if (entity.Level.IsIZombie())
            {
                Global.AddSaveStat(VanillaStats.CATEGORY_IZ_ENEMY_DEATH, entity.GetDefinitionID(), 1);
            }
        }
        private void PostGameOverCallback(LevelCallbacks.PostGameOverParams param, CallbackResult result)
        {
            var killer = param.killer;
            var level = param.level;
            if (level.IsIZombie())
            {
                Global.AddSaveStat(VanillaStats.CATEGORY_IZ_GAME_OVER, level.StageID, 1);
            }
            else
            {
                if (killer != null)
                {
                    Global.AddSaveStat(VanillaStats.CATEGORY_ENEMY_GAME_OVER, killer.GetDefinitionID(), 1);
                }
            }
        }
    }
}
