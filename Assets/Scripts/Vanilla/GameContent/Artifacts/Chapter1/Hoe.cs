using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Grids;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.hoe)]
    public class Hoe : ArtifactDefinition
    {
        public Hoe(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_LEVEL_START, PostLevelStartCallback);
            AddTrigger(LevelCallbacks.POST_ENEMY_SPAWNED, PostEnemySpawnedCallback);
        }
        private void PostLevelStartCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                if (artifact.Definition.GetID() != VanillaArtifactID.hoe)
                    continue;
                artifact.SetInactive(false);
            }
        }
        private void PostEnemySpawnedCallback(EntityCallbackParams param, CallbackResult result)
        {
            var enemy = param.entity;
            var level = enemy.Level;
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                if (artifact.Definition.GetID() != VanillaArtifactID.hoe)
                    continue;
                if (artifact.IsInactive())
                    continue;
                var targetLane = enemy.GetLane();
                var targetColumn = TARGET_COLUMN;
                var grid = level.GetGrid(targetColumn, targetLane);
                if (grid == null)
                    continue;
                if (grid.Definition.GetID() == VanillaGridID.water)
                    continue;
                var pos = grid.GetEntityPosition();
                var hoe = level.Spawn(VanillaEffectID.hoe, pos, null);
                var smoke = level.Spawn(VanillaEffectID.smoke, pos, null);
                smoke.SetSize(hoe.GetSize());
                artifact.Highlight();
                artifact.SetInactive(true);
                break;
            }
        }
        public const int TARGET_COLUMN = 3;
    }
}
