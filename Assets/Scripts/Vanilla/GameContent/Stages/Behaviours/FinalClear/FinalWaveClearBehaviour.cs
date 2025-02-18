using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    public class FinalWaveClearBehaviour : StageBehaviour
    {
        public FinalWaveClearBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }

        public override void Update(LevelEngine level)
        {
            switch (level.WaveState)
            {
                case VanillaLevelStates.STATE_FINAL_WAVE:
                    CheckNoProduction(level);
                    level.CheckClearUpdate();
                    break;
            }
        }
        private void CheckNoProduction(LevelEngine level)
        {
            var behaviour = level.GetStageBehaviour<WaveStageBehaviour>();
            if (behaviour != null)
            {
                var waveTimer = behaviour.GetWaveTimer(level);
                waveTimer.Run();
                if (waveTimer.Expired)
                {
                    level.SetNoEnergy(true);
                }
            }
        }
    }
}
