using System.Linq;
using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

namespace MVZ2.GameContent.Stages
{
    public class NightmareStageBehaviour : BossStageBehaviour
    {
        public NightmareStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        protected override void AfterFinalWaveUpdate(LevelEngine level)
        {
            base.AfterFinalWaveUpdate(level);
            var waveTimer = GetWaveTimer(level);

            // 让眼睛闭眼。
            foreach (var eye in level.FindEntities(VanillaEffectID.nightmareWatchingEye))
            {
                if (eye.Timeout < 0)
                {
                    eye.Timeout = 30;
                }
            }

            // 音乐放缓。
            level.SetMusicVolume(Mathf.Clamp01(level.GetMusicVolume() - (1 / 30f)));

            //waveTimer.Run();
            //if (waveTimer.Expired)
            //{
            //    level.WaveState = STATE_BOSS_FIGHT;
            //    var frankenstein = level.Spawn(VanillaBossID.frankenstein, targetEnemy.Position, targetEnemy);
            //    foreach (var ent in level.FindEntities(e => !e.IsDead && e.HasBuff<FrankensteinTransformerBuff>()))
            //    {
            //        ent.Remove();
            //    }
            //    Frankenstein.DoTransformationEffects(frankenstein);
            //    // 音乐。
            //    level.PlayMusic(VanillaMusicID.halloweenBoss);
            //    level.SetMusicVolume(1);
            //    // 血条。
            //    level.SetProgressBarToBoss(VanillaProgressBarID.frankenstein);
            //    // 重置下一波计时器。
            //    waveTimer.ResetTime(200);
            //}
        }
        protected override void BossFightWaveUpdate(LevelEngine level)
        {
            base.BossFightWaveUpdate(level);
            // 如果不存在Boss，或者所有Boss死亡，进入BOSS后阶段。
            // 如果有Boss存活，不停生成怪物。
            var targetBosses = level.FindEntities(e => e.Type == EntityTypes.BOSS && e.IsHostileEnemy());
            if (targetBosses.Length <= 0 || targetBosses.All(b => b.IsDead))
            {
                level.WaveState = STATE_AFTER_BOSS;
                level.StopMusic();
            }
            else
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
        }
        protected override void AfterBossWaveUpdate(LevelEngine level)
        {
            base.AfterBossWaveUpdate(level);
            foreach (var entity in level.FindEntities(e => !e.IsDead && e.IsHostileEnemy()))
            {
                entity.Die();
            }
        }
    }
}
