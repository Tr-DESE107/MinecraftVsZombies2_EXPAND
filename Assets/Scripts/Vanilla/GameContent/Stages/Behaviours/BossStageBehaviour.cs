using System.Linq;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    public abstract class BossStageBehaviour : WaveStageBehaviourBase
    {
        public BossStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        public override void Update(LevelEngine level)
        {
            base.Update(level);
            switch (level.WaveState)
            {
                case STATE_AFTER_FINAL_WAVE:
                    AfterFinalWaveUpdate(level);
                    break;
                case STATE_BOSS_FIGHT:
                    BossFightWaveUpdate(level);
                    break;
                case STATE_AFTER_BOSS:
                    AfterBossWaveUpdate(level);
                    break;
            }
        }
        protected override void FinalWaveUpdate(LevelEngine level)
        {
            base.FinalWaveUpdate(level);
            var lastEnemy = level.FindEntities(e => e.IsAliveEnemy()).FirstOrDefault();
            if (lastEnemy == null)
            {
                level.WaveState = STATE_AFTER_FINAL_WAVE;
                level.PostWaveFinished(level.CurrentWave);
            }
        }
        protected virtual void AfterFinalWaveUpdate(LevelEngine level)
        {

        }
        protected virtual void BossFightWaveUpdate(LevelEngine level)
        {

        }
        protected virtual void AfterBossWaveUpdate(LevelEngine level)
        {

        }
    }
}
