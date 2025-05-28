﻿using System.Linq;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class FrankensteinStageBehaviour : BossStageBehaviour
    {
        public FrankensteinStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        protected override void AfterFinalWaveUpdate(LevelEngine level)
        {
            base.AfterFinalWaveUpdate(level);
            var frankensteinTimer = GetOrCreateFrankensteinTimer(level);

            // 如果没有科学怪人的目标，再生成一个。
            var targetEnemy = level.FindFirstEntity(e => !e.IsDead && e.HasBuff<FrankensteinTransformerBuff>());
            if (targetEnemy == null || !targetEnemy.Exists())
            {
                var position = new Vector3(VanillaLevelExt.ENEMY_RIGHT_BORDER, 0, level.GetEntityLaneZ(Mathf.FloorToInt(level.GetMaxLaneCount() * 0.5f)));
                var enemy = level.Spawn(VanillaEnemyID.zombie, position, null);
                enemy.AddBuff<FrankensteinTransformerBuff>();
                frankensteinTimer.ResetTime(300);
            }


            // 音乐放缓。
            level.SetMusicVolume(Mathf.Clamp01(level.GetMusicVolume() - (1 / 30f)));

            frankensteinTimer.Run();
            if (frankensteinTimer.Expired)
            {
                level.WaveState = VanillaLevelStates.STATE_BOSS_FIGHT;
                var frankenstein = level.Spawn(VanillaBossID.frankenstein, targetEnemy.Position, targetEnemy);
                foreach (var ent in level.FindEntities(e => !e.IsDead && e.HasBuff<FrankensteinTransformerBuff>()))
                {
                    ent.Remove();
                }
                Frankenstein.DoTransformationEffects(frankenstein);
                // 音乐。
                level.PlayMusic(VanillaMusicID.halloweenBoss);
                level.SetMusicVolume(1);
                // 血条。
                level.SetProgressBarToBoss(VanillaProgressBarID.frankenstein);
                // 重置下一波计时器。
                var behaviour = level.GetStageBehaviour<WaveStageBehaviour>();
                if (behaviour != null)
                {
                    var timer = WaveStageBehaviour.GetWaveTimer(level);
                    timer.ResetTime(200);
                }
                frankensteinTimer.ResetTime(300);
            }
        }
        protected override void BossFightWaveUpdate(LevelEngine level)
        {
            base.BossFightWaveUpdate(level);
            // 如果不存在Boss，继续播放传送带关音乐，进入到Boss后阶段
            // 如果所有Boss死亡，音乐放缓
            // 如果有Boss存活，不停生成怪物。
            var targetBosses = level.FindEntities(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity());
            if (targetBosses.Length <= 0)
            {
                level.WaveState = VanillaLevelStates.STATE_AFTER_BOSS;
                level.PlayMusic(level.GetMusicID());
                level.SetMusicVolume(1);
                level.SetProgressBarToStage();
            }
            else if (targetBosses.All(b => b.IsDead))
            {
                level.SetMusicVolume(Mathf.Clamp01(level.GetMusicVolume() - (1 / 30f)));
                level.SetLastEnemyPosition(targetBosses.First().Position);
            }
            else
            {
                RunBossWave(level);
            }
        }
        protected override void AfterBossWaveUpdate(LevelEngine level)
        {
            base.AfterBossWaveUpdate(level);
            level.CheckClearUpdate();
        }
        public FrameTimer GetFrankentsteinTimer(LevelEngine level) => level.GetBehaviourField<FrameTimer>(PROP_FRANKENSTEIN_TIMER);
        public void SetFrankensteinTimer(LevelEngine level, FrameTimer value) => level.SetBehaviourField(PROP_FRANKENSTEIN_TIMER, value);
        public FrameTimer GetOrCreateFrankensteinTimer(LevelEngine level)
        {
            var roundTimer = GetFrankentsteinTimer(level);
            if (roundTimer == null)
            {
                roundTimer = new FrameTimer(300);
                SetFrankensteinTimer(level, roundTimer);
            }
            return roundTimer;
        }
        private const string PROP_REGION = "frankenstein_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_FRANKENSTEIN_TIMER = new VanillaLevelPropertyMeta<FrameTimer>("FrankensteinTimer");
    }
}
