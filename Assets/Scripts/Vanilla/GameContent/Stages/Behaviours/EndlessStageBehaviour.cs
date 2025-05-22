using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class EndlessStageBehaviour : StageBehaviour
    {
        public EndlessStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            stageDef.AddTrigger(LevelCallbacks.POST_WAVE_FINISHED, PostWaveFinishedCallback);
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
            }
        }
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);
            if (level.IsFinalWave(wave))
            {
                var roundTimer = GetOrCreateRoundTimer(level);
                roundTimer.ResetTime(1800);
            }
        }
        private void PostWaveFinishedCallback(LevelCallbacks.PostWaveParams param, CallbackResult result)
        {
            var level = param.level;
            if (!level.IsEndless())
                return;
            if (Global.GetSaveStat(VanillaStats.CATEGORY_MAX_ENDLESS_FLAGS, level.StageID) < level.CurrentFlag)
            {
                Global.SetSaveStat(VanillaStats.CATEGORY_MAX_ENDLESS_FLAGS, level.StageID, level.CurrentFlag);
            }
        }
        #region 更新关卡
        protected virtual void FinalWaveUpdate(LevelEngine level)
        {
            var lastEnemy = level.FindEntities(e => e.IsAliveEnemy()).FirstOrDefault();
            var roundTimer = GetOrCreateRoundTimer(level);
            roundTimer.Run();
            if (roundTimer.Expired || lastEnemy == null)
            {
                level.PostWaveFinished(level.CurrentWave);
                level.WaveState = VanillaLevelStates.STATE_AFTER_FINAL_WAVE;
                roundTimer.ResetTime(150);
                level.PlaySound(VanillaSoundID.hugeWave);
                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_MORE_ENEMIES_APPROACHING, 1000, 150);
            }
        }
        protected virtual void AfterFinalWaveUpdate(LevelEngine level)
        {
            var roundTimer = GetOrCreateRoundTimer(level);
            if (roundTimer.Frame == 1)
            {
                roundTimer.Run();
                level.SaveStateData();
            }
            roundTimer.Run();
            if (roundTimer.Expired)
            {
                level.StopLevel();
                level.SetEnemyPool(GenerateEnemyPool(level, level.CurrentFlag));
                level.CurrentWave = 0;
                level.HideAdvice();
                level.BeginLevel();
            }
        }
        #endregion

        protected virtual NamespaceID[] GenerateEnemyPool(LevelEngine level, int flag)
        {
            List<NamespaceID> entries = new List<NamespaceID>()
            {
                VanillaSpawnID.zombie,
                VanillaSpawnID.leatherCappedZombie,
            };
            int round = flag / 2;
            if (round == 0)
            {

            }
            if (round == 1)
            {
                entries.Add(VanillaSpawnID.ironHelmettedZombie);
            }
            else
            {
                int maxEnemyTypeCount = Mathf.Min(round + 1, 9);

                if (round >= 5)
                {
                    entries.Add(VanillaSpawnID.mutantZombie);
                    maxEnemyTypeCount--;
                }
                if (round >= 10)
                {
                    entries.Add(VanillaSpawnID.megaMutantZombie);
                    maxEnemyTypeCount--;
                }

                var game = Global.Game;
                NamespaceID[] enemies = game.GetUnlockedEnemies();

                var areaDef = level.AreaDefinition;
                var validEnemies = enemies
                    .Select(e => VanillaSpawnID.GetFromEntity(e))
                    .Where(spawnID =>
                    {
                        if (spawnID == VanillaSpawnID.mutantZombie || spawnID == VanillaSpawnID.megaMutantZombie)
                            return false;
                        var spawnDef = game.GetSpawnDefinition(spawnID);
                        if (spawnDef == null || spawnDef.SpawnLevel <= 0)
                            return false;
                        if (spawnDef.NoEndless)
                            return false;
                        if (entries.Any(id => id == spawnID))
                            return false;
                        var excludedAreaTags = spawnDef.ExcludedAreaTags;
                        if (areaDef.GetAreaTags().Any(t => excludedAreaTags.Contains(t)))
                            return false;
                        return true;
                    })
                    .RandomTake(maxEnemyTypeCount, level.GetRoundRNG());
                entries.AddRange(validEnemies);
            }
            return entries.ToArray();
        }
        public FrameTimer GetRoundTimer(LevelEngine level) => level.GetBehaviourField<FrameTimer>(PROP_ROUND_TIMER);
        public void SetRoundTimer(LevelEngine level, FrameTimer value) => level.SetBehaviourField(PROP_ROUND_TIMER, value);
        public FrameTimer GetOrCreateRoundTimer(LevelEngine level)
        {
            var roundTimer = GetRoundTimer(level);
            if (roundTimer == null)
            {
                roundTimer = new FrameTimer(1800);
                SetRoundTimer(level, roundTimer);
            }
            return roundTimer;
        }
        private const string PROP_REGION = "endless_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta PROP_ROUND_TIMER = new VanillaLevelPropertyMeta("RoundTimer");
    }
}
