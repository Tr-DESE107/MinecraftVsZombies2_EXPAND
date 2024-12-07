using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Implements
{
    public class StatsImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_PLACE, PostPlaceContraptionCallback);
            mod.AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_DESTROY, PostContraptionDestroyCallback);

            mod.AddTrigger(LevelCallbacks.POST_ENEMY_SPAWNED, PostEnemySpawnedCallback);
            mod.AddTrigger(VanillaLevelCallbacks.POST_ENEMY_NEUTRALIZE, PostEnemyNeutralizeCallback);
            mod.AddTrigger(LevelCallbacks.POST_GAME_OVER, PostGameOverCallback);
        }
        private void PostPlaceContraptionCallback(Entity contraption)
        {
            Global.AddSaveStat(VanillaStats.CATEGORY_CONTRAPTION_PLACE, contraption.GetDefinitionID(), 1);
        }
        private void PostContraptionDestroyCallback(Entity contraption)
        {
            Global.AddSaveStat(VanillaStats.CATEGORY_CONTRAPTION_DESTROY, contraption.GetDefinitionID(), 1);
        }
        private void PostEnemySpawnedCallback(Entity enemy)
        {
            Global.AddSaveStat(VanillaStats.CATEGORY_ENEMY_SPAWN, enemy.GetDefinitionID(), 1);
        }
        private void PostEnemyNeutralizeCallback(Entity enemy)
        {
            Global.AddSaveStat(VanillaStats.CATEGORY_ENEMY_NEUTRALIZE, enemy.GetDefinitionID(), 1);
        }
        private void PostGameOverCallback(LevelEngine level, int type, Entity killer, string message)
        {
            if (killer == null)
                return;
            Global.AddSaveStat(VanillaStats.CATEGORY_ENEMY_GAME_OVER, killer.GetDefinitionID(), 1);
        }
    }
}
