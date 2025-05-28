﻿using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class GiantStageBehaviour : BossStageBehaviour
    {
        public GiantStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        protected override void AfterFinalWaveUpdate(LevelEngine level)
        {
            base.AfterFinalWaveUpdate(level);
            TransitionUpdate(level);
        }
        private void TransitionUpdate(LevelEngine level)
        {
            if (level.EntityExists(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity() && !e.IsDead))
            {
                // 瘦长鬼影出现
                level.WaveState = VanillaLevelStates.STATE_BOSS_FIGHT;
                return;
            }
            if (!level.HasBuff<TheGiantTransitionBuff>())
            {
                level.AddBuff<TheGiantTransitionBuff>();
            }
        }
        protected override void BossFightWaveUpdate(LevelEngine level)
        {
            base.BossFightWaveUpdate(level);
            var state = GetBossState(level);
            switch (state)
            {
                case BOSS_STATE_PHASE1_2:
                    TheGiantUpdate(level);
                    break;
                case BOSS_STATE_PHASE3_TRANSITION:
                    TheGiantPhase3TransitionUpdate(level);
                    break;
                case BOSS_STATE_PHASE3:
                    TheGiantPhase3Update(level);
                    break;
            }
        }
        private void TheGiantUpdate(LevelEngine level)
        {
            // 巨人1、2阶段战斗
            // 如果不存在Boss，或者所有Boss死亡，进入BOSS后阶段。
            // 如果有Boss存活，不停生成怪物。
            var phase1Boss = level.EntityExists(e => e.IsEntityOf(VanillaBossID.theGiant) && e.IsHostileEntity() && !e.IsDead && TheGiant.GetPhase(e) == TheGiant.PHASE_1);
            var phase2Boss = level.EntityExists(e => e.IsEntityOf(VanillaBossID.theGiant) && e.IsHostileEntity() && !e.IsDead && TheGiant.GetPhase(e) == TheGiant.PHASE_2);
            WaveStageBehaviour.SetHighWave(level, phase2Boss);
            if (!phase1Boss && !phase2Boss)
            {
                SetBossState(level, BOSS_STATE_PHASE3_TRANSITION);
                // 隐藏UI，关闭输入
                level.StopMusic();
            }
            else
            {
                RunBossWave(level);
            }
        }
        private void TheGiantPhase3TransitionUpdate(LevelEngine level)
        {
            ClearEnemies(level);
            if (level.EntityExists(e => e.IsEntityOf(VanillaBossID.theGiant) && e.IsHostileEntity() && !e.IsDead && TheGiant.GetPhase(e) == TheGiant.PHASE_3))
            {
                // 巨人3阶段出现
                level.PlayMusic(VanillaMusicID.mausoleumBoss2);
                SetBossState(level, BOSS_STATE_PHASE3);
                return;
            }
        }
        private void TheGiantPhase3Update(LevelEngine level)
        {
            // 梦魇收割者战斗
            // 如果不存在Boss，或者所有Boss死亡，进入BOSS后阶段。
            // 如果有Boss存活，不停生成怪物。
            if (!level.EntityExists(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity() && !e.IsDead))
            {
                level.WaveState = VanillaLevelStates.STATE_AFTER_BOSS;
                level.StopMusic();
                if (!level.IsRerun)
                {
                    // 隐藏UI，关闭输入
                    level.ResetHeldItem();
                    level.SetUIAndInputDisabled(true);
                }
                else
                {
                    var giant = level.FindFirstEntity(VanillaBossID.theGiant);
                    Vector3 position;
                    if (giant != null)
                    {
                        position = giant.Position;
                    }
                    else
                    {
                        var x = level.GetLawnCenterX();
                        var z = level.GetLawnCenterZ();
                        var y = level.GetGroundY(x, z);
                        position = new Vector3(x, y, z);
                    }
                    level.Produce(VanillaPickupID.clearPickup, position, null);
                }
            }
            else
            {
                RunBossWave(level);
            }
        }
        protected override void AfterBossWaveUpdate(LevelEngine level)
        {
            base.AfterBossWaveUpdate(level);
            ClearEnemies(level);
            if (!level.IsRerun)
            {
                if (!level.IsCleared)
                {
                    if (!level.EntityExists(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity() && !e.IsDead))
                    {
                        if (!level.HasBuff<TheGiantClearedBuff>())
                        {
                            level.AddBuff<TheGiantClearedBuff>();
                        }
                    }
                }
            }
        }

        public const int BOSS_STATE_PHASE1_2 = 0;
        public const int BOSS_STATE_PHASE3_TRANSITION = 1;
        public const int BOSS_STATE_PHASE3 = 2;
    }
}
