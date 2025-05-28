using MVZ2.Vanilla.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;

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
