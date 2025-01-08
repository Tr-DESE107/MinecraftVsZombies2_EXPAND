using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
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
    public class EndlessStageBehaviour : WaveStageBehaviourBase
    {
        public EndlessStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            stageDef.AddTrigger(LevelCallbacks.POST_WAVE_FINISHED, PostWaveFinishedCallback);
        }
        public override void Update(LevelEngine level)
        {
            base.Update(level);
            if (level.WaveState == STATE_AFTER_FINAL_WAVE)
            {
                AfterFinalWaveUpdate(level);
            }
        }
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);
            if (level.IsFinalWave(wave))
            {
                var waveTimer = GetWaveTimer(level);
                waveTimer.ResetTime(1800);
            }
        }
        private void PostWaveFinishedCallback(LevelEngine level, int wave)
        {
            if (!level.IsEndless())
                return;
            if (Global.GetSaveStat(VanillaStats.CATEGORY_MAX_ENDLESS_FLAGS, level.StageID) < level.CurrentFlag)
            {
                Global.SetSaveStat(VanillaStats.CATEGORY_MAX_ENDLESS_FLAGS, level.StageID, level.CurrentFlag);
            }
        }

        protected override bool ShouldTriggerFinalWaveEvent(LevelEngine level)
        {
            return false;
        }
        #region 更新关卡
        protected override void FinalWaveUpdate(LevelEngine level)
        {
            // 这里不继承原先的FinalWaveUpdate，因为无尽模式不会触发最后一波事件。
            var lastEnemy = level.FindEntities(e => e.IsAliveEnemy()).FirstOrDefault();
            var waveTimer = GetWaveTimer(level);
            waveTimer.Run();
            if (waveTimer.Expired || lastEnemy == null)
            {
                level.PostWaveFinished(level.CurrentWave);
                level.WaveState = STATE_AFTER_FINAL_WAVE;
                waveTimer.ResetTime(150);
                level.PlaySound(VanillaSoundID.hugeWave);
                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_MORE_ENEMIES_APPROACHING, 1000, 150);
            }
        }
        protected virtual void AfterFinalWaveUpdate(LevelEngine level)
        {
            var waveTimer = GetWaveTimer(level);
            if (waveTimer.Frame == 1)
            {
                waveTimer.Run();
                level.SaveStateData();
            }
            waveTimer.Run();
            if (waveTimer.Expired)
            {
                level.StopLevel();
                level.SetEnemyPool(GenerateEnemyPool(level, level.CurrentFlag));
                level.ClearSeedPacks();
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

                var game = Global.Game;
                NamespaceID[] enemies = game.GetUnlockedEnemies();

                var areaDef = level.AreaDefinition;
                var validEnemies = enemies.Select(e => VanillaSpawnID.GetFromEntity(e)).Where(spawnID =>
                {
                    var spawnDef = game.GetSpawnDefinition(spawnID);
                    if (spawnDef == null || spawnDef.SpawnLevel <= 0)
                        return false;
                    if (entries.Any(id => id == spawnID))
                        return false;
                    var excludedAreaTags = spawnDef.ExcludedAreaTags;
                    if (areaDef.GetAreaTags().Any(t => excludedAreaTags.Contains(t)))
                        return false;
                    return true;
                }).RandomTake(maxEnemyTypeCount, level.GetRoundRNG());
                entries.AddRange(validEnemies);
            }
            return entries.ToArray();
        }
    }
}
