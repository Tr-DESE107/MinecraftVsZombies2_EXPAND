using System.Linq;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    public abstract class BossStageBehaviour : StageBehaviour
    {
        public BossStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        public override void Update(LevelEngine level)
        {
            base.Update(level);
            switch (level.WaveState)
            {
                case VanillaLevelStates.STATE_FINAL_WAVE:
                    FinalWaveUpdate(level);
                    break;
                case VanillaLevelStates.STATE_AFTER_FINAL_WAVE:
                    AfterFinalWaveUpdate(level);
                    break;
                case VanillaLevelStates.STATE_BOSS_FIGHT:
                    BossFightWaveUpdate(level);
                    break;
                case VanillaLevelStates.STATE_AFTER_BOSS:
                    AfterBossWaveUpdate(level);
                    break;
            }
        }
        protected virtual void FinalWaveUpdate(LevelEngine level)
        {
            var lastEnemy = level.FindEntities(e => e.IsAliveEnemy()).FirstOrDefault();
            if (lastEnemy == null)
            {
                StartAfterFinalWave(level);
            }
        }
        protected virtual void StartAfterFinalWave(LevelEngine level)
        {
            level.WaveState = VanillaLevelStates.STATE_AFTER_FINAL_WAVE;
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
        protected void RunBossWave(LevelEngine level)
        {
            var behaviour = level.GetStageBehaviour<WaveStageBehaviour>();
            if (behaviour != null)
            {
                behaviour.RunBossWave(level);
            }
        }
        protected void ClearEnemies(LevelEngine level)
        {
            foreach (var entity in level.FindEntities(e => e.Type == EntityTypes.ENEMY && !e.IsDead && e.IsHostileEntity()))
            {
                entity.Die();
            }
        }
        protected static void SetBossState(LevelEngine level, int value) => level.SetBehaviourField(FIELD_BOSS_PHASE, value);
        protected static int GetBossState(LevelEngine level) => level.GetBehaviourField<int>(FIELD_BOSS_PHASE);
        private const string PROP_REGION = "boss_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        private static readonly VanillaLevelPropertyMeta FIELD_BOSS_PHASE = new VanillaLevelPropertyMeta("BossPhase");
    }
}
