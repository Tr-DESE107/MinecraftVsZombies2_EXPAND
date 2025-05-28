﻿using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class WitherStageBehaviour : BossStageBehaviour
    {
        public WitherStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        protected override void AfterFinalWaveUpdate(LevelEngine level)
        {
            base.AfterFinalWaveUpdate(level);
            WitherTransitionUpdate(level);
        }
        private void WitherTransitionUpdate(LevelEngine level)
        {
            if (!level.HasBuff<WitherTransitionBuff>())
            {
                level.AddBuff<WitherTransitionBuff>();
            }
        }
        protected override void BossFightWaveUpdate(LevelEngine level)
        {
            base.BossFightWaveUpdate(level);
            WitherFightUpdate(level);
        }

        private void WitherFightUpdate(LevelEngine level)
        {
            // 凋灵战斗
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
                    var reaper = level.FindFirstEntity(VanillaBossID.wither);
                    Vector3 position;
                    if (reaper != null)
                    {
                        position = reaper.Position;
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
                if (!level.IsCleared && !level.EntityExists(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity()))
                {
                    if (!level.HasBuff<WitherClearedBuff>())
                    {
                        level.AddBuff<WitherClearedBuff>();
                    }
                }
            }
        }
    }
}
