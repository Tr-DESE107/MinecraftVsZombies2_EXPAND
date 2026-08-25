#nullable enable  

using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Localization;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Definitions;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Localization;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Artifacts  
{  
    [AutoArtifactDefinition(VanillaArtifactNames.InfiniteCore)]  
    public class InfiniteCore : ArtifactDefinition  
    {
        public InfiniteCore(string nsp, string name) : base(nsp, name)
        {
            // 修正：CALCULATE_SPAWN_POINTS（不是 POST_CALCULATE_SPAWN_POINT）  
            AddTrigger(LogicLevelCallbacks.CALCULATE_SPAWN_POINTS, ClampSpawnPointCallback);
        }

        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            var level = artifact.Level;
            artifact.SetGlowing(true);

            // ── 计数：每帧根据 CurrentWave 推算“第几波额外波” ──  
            int totalWaves = level.GetTotalWaveCount();
            int newExtraWave = level.CurrentWave - totalWaves;
            if (newExtraWave < 0)
                newExtraWave = 0;

            // 关键：先取旧值，只有数字“真正发生变化”时才 Highlight 一次。  
            // GetNumber() 读的是上一帧写入的值（属性 NUMBER 会持久化），  
            // 因此“新值 != 旧值”即代表这一帧数字变了，闪一次刚好对应“每出一波额外波”。  
            int oldExtraWave = artifact.GetNumber();
            if (newExtraWave != oldExtraWave)
            {
                artifact.SetNumber(newExtraWave);

                // 仅在数字变化的这一帧触发一次高亮
                if (newExtraWave > 0)
                {
                    artifact.Highlight();
                    level.PlaySound(VanillaSoundID.hugeWave);
                }
            }

            if (level.WaveState == VanillaLevelStates.STATE_FINAL_WAVE)
            {
                level.WaveState = VanillaLevelStates.STATE_STARTED;

                level.PlaySound(VanillaSoundID.hugeWave);
                level.ShowAdvice(LogicStrings.CONTEXT_ADVICE,
                    VanillaStrings.ADVICE_MORE_ENEMIES_APPROACHING, 1000, 150);
            }

            // ── 永不通关：与计数解耦，单独处理状态翻转 ──  
            if (level.WaveState == VanillaLevelStates.STATE_FINAL_WAVE)
            {
                level.WaveState = VanillaLevelStates.STATE_STARTED;
            }
        }

        private void ClampSpawnPointCallback(
            LogicLevelCallbacks.CalculateSpawnPointParams param, CallbackResult result)
        {
            if (!param.level.HasArtifact(GetID()))
                return;
            // 修正：LevelSpawnPointParams 用 basePoints / multiplier，没有 pointMax  
            param.param.basePoints = Mathf.Min(param.param.basePoints, MAX_BASE_POINTS);
            param.param.multiplier = Mathf.Min(param.param.multiplier, MAX_MULTIPLIER);
        }

        public const float MAX_BASE_POINTS = 60f;
        public const float MAX_MULTIPLIER = 3f;

    }  
}
