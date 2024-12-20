using System.Linq;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

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
