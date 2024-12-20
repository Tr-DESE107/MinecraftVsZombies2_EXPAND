using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    public class WaveStageBehaviour : WaveStageBehaviourBase
    {
        public WaveStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }

        #region 更新关卡
        protected override void FinalWaveUpdate(LevelEngine level)
        {
            base.FinalWaveUpdate(level);
            CheckClearUpdate(level);
        }
        #endregion
    }
}
