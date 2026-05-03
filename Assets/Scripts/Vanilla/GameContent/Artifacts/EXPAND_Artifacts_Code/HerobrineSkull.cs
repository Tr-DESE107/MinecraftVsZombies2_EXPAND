using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using MVZ2.GameContent.Enemies;
using MVZ2Logic.Definitions;
using MVZ2Logic.Callbacks;

namespace MVZ2.GameContent.Artifacts
{
    [AutoArtifactDefinition(VanillaArtifactNames.HerobrineSkull)]
    public class HerobrineSkull : ArtifactDefinition
    {
        public HerobrineSkull(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LogicLevelCallbacks.POST_WAVE_ENEMY_SPAWN, PostWaveEnemySpawnCallback);
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
                    // ����������ɶ������
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