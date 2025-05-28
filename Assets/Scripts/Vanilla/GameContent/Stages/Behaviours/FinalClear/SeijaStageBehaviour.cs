﻿using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.GameContent.Talk;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
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
            stageDef.AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_EVOKE, PostGravityPadEvokeCallback, filter: VanillaContraptionID.gravityPad);
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
                level.SimpleStartTalk(VanillaTalkID.castle7Boss, 0, delay: 1, onEnd: onEnd);
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
                SpawnMesmerizer(level, new Vector3(x, y, z));
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
            level.SetNoEnergy(false);
            // 音乐。
            level.PlayMusic(VanillaMusicID.seija);
            // 血条。
            level.SetProgressBarToBoss(VanillaProgressBarID.seija);
            // 重置下一波计时器。
            var behaviour = level.GetStageBehaviour<WaveStageBehaviour>();
            if (behaviour != null)
            {
                var timer = WaveStageBehaviour.GetWaveTimer(level);
                timer.ResetTime(200);
            }
        }
        private Entity SpawnMesmerizer(LevelEngine level, Vector3 position)
        {
            if (level.GetProperty<bool>(FIELD_MESMERIZER_SPAWNED))
                return null;
            level.SetProperty(FIELD_MESMERIZER_SPAWNED, true);
            var entity = level.Spawn(VanillaEnemyID.mesmerizer, position, null);
            entity.AddBuff<SeijaMesmerizerBuff>();
            return entity;
        }
        private void PostGravityPadEvokeCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;
            var level = contraption.Level;
            if (!level.HasBehaviour<SeijaStageBehaviour>())
                return;
            if (level.WaveState != VanillaLevelStates.STATE_BOSS_FIGHT)
                return;
            var x = contraption.Position.x;
            var z = contraption.Position.z;
            var y = 800;
            SpawnMesmerizer(level, new Vector3(x, y, z));
        }
        public const string PROP_REGION = "seija_stage_behaviour";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<bool> FIELD_MESMERIZER_SPAWNED = new VanillaLevelPropertyMeta<bool>("MesmerizerSpawned");
    }
}
