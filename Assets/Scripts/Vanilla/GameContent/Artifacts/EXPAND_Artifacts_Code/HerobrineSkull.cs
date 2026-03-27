using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using MVZ2.GameContent.Enemies;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.HerobrineSkull)]
    public class HerobrineSkull : ArtifactDefinition
    {
        public HerobrineSkull(string nsp, string name) : base(nsp, name)
        {
            // 注册 POST_WAVE_ENEMY_SPAWN 回调  
            AddTrigger(VanillaLevelCallbacks.POST_WAVE_ENEMY_SPAWN, PostWaveEnemySpawnCallback);
        }

        private void PostWaveEnemySpawnCallback(
            VanillaLevelCallbacks.WaveEnemySpawnParams param,
            CallbackResult result)
        {
            var level = param.level;
            var wave = param.wave;

            foreach (var artifact in level.GetArtifacts())
            {
                if (artifact?.Definition != this)
                    continue;

                var rng = artifact.RNG;
                if (rng.Next(10) < 7)
                {
                    // 在随机行生成额外敌人
                    switch (rng.Next(4))
                    {
                        case 0:
                        default:
                            level.SpawnEnemyAtRandomLane(VanillaEnemyID.Mannequin);
                            break;
                        case 1:
                            level.SpawnEnemyAtRandomLane(VanillaEnemyID.ImpMannequin);
                            break;
                        case 2:
                            level.SpawnEnemyAtRandomLane(VanillaEnemyID.MegaImpMannequin);
                            break;
                        case 3:
                            level.SpawnEnemyAtRandomLane(VanillaEnemyID.MutantMannequin);
                            break;
                    }
                    
                    artifact.Highlight();
                }
            }
        }
    }
}