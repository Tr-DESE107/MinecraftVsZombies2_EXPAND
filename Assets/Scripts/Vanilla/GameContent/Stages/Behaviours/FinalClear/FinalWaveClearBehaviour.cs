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
                    level.CheckClearUpdate();
                    break;
            }
        }
    }
}
