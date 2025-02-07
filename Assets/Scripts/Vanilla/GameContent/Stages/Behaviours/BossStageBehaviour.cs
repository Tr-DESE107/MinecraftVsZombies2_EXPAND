using System.Linq;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

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
                StartAfterFinalWave(level);
            }
        }
        protected virtual void StartAfterFinalWave(LevelEngine level)
        {
            level.WaveState = STATE_AFTER_FINAL_WAVE;
            level.PostWaveFinished(level.CurrentWave);
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
        protected void RunWave(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            CheckWaveAdvancement(level);
            if (waveTimer.Expired)
            {
                SetWaveMaxHealth(level, 0);
                waveTimer.ResetTime(level.GetWaveMaxTime());
                level.RunWave();
            }
        }
        protected static void SetBossState(LevelEngine level, int value) => level.SetBehaviourField(FIELD_BOSS_PHASE, value);
        protected static int GetBossState(LevelEngine level) => level.GetBehaviourField<int>(FIELD_BOSS_PHASE);
        private const string PROP_REGION = "boss_stage_behaviour";
        [PropertyRegistry(PROP_REGION)]
        private static readonly VanillaLevelPropertyMeta FIELD_BOSS_PHASE = new VanillaLevelPropertyMeta("BossPhase");
    }
}
