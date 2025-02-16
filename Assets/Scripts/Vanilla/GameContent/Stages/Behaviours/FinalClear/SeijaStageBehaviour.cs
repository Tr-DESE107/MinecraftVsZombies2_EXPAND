using System.Linq;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.GameContent.Talk;
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
    public class SeijaStageBehaviour : BossStageBehaviour
    {
        public SeijaStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        protected override void StartAfterFinalWave(LevelEngine level)
        {
            base.StartAfterFinalWave(level);
            if (level.IsRerun)
            {
                StartBattle(level);
            }
            else
            {
                level.AddBuff<SeijaAutoCollectBuff>();
                level.SimpleStartTalk(VanillaTalkID.castle7Boss, 0, delay: 1, onSkipped: onEnd, onEnd: onEnd);
                void onEnd()
                {
                    StartBattle(level);
                    level.RemoveBuffs<SeijaAutoCollectBuff>();
                }
            }
        }
        protected override void BossFightWaveUpdate(LevelEngine level)
        {
            base.BossFightWaveUpdate(level);
            // 如果不存在Boss，继续播放音乐，进入到Boss后阶段
            // 如果所有Boss死亡，音乐放缓
            // 如果有Boss存活，不停生成怪物。
            var targetBosses = level.FindEntities(e => e.Type == EntityTypes.BOSS && e.IsHostileEntity() && e.ExistsAndAlive());
            if (targetBosses.Length <= 0)
            {
                level.WaveState = VanillaLevelStates.STATE_AFTER_BOSS;
                level.StopMusic();
                level.SetProgressBarToStage();
                var x = level.GetEntityColumnX(level.GetMaxColumnCount() - 2);
                var z = level.GetEntityLaneZ(level.GetMaxLaneCount() / 2);
                var y = 800;
                level.Spawn(VanillaEnemyID.mesmerizer, new Vector3(x, y, z), null);
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
        private void StartBattle(LevelEngine level)
        {
            level.WaveState = VanillaLevelStates.STATE_BOSS_FIGHT;
            var x = VanillaLevelExt.ENEMY_RIGHT_BORDER;
            var z = level.GetEntityLaneZ(level.GetMaxLaneCount() / 2);
            var y = level.GetGroundY(x, z);
            var seija = level.Spawn(VanillaBossID.seija, new Vector3(x, y, z), null);
            Seija.StartState(seija, VanillaEntityStates.SEIJA_FRONTFLIP);
            level.SetNoProduction(false);
            // 音乐。
            level.PlayMusic(VanillaMusicID.seija);
            // 血条。
            level.SetProgressBarToBoss(VanillaProgressBarID.seija);
            // 重置下一波计时器。
            var behaviour = level.GetStageBehaviour<WaveStageBehaviour>();
            if (behaviour != null)
            {
                var timer = behaviour.GetWaveTimer(level);
                timer.ResetTime(200);
            }
        }
    }
}
