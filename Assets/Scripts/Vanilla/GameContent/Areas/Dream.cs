using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(VanillaAreaNames.dream)]
    public class Dream : AreaDefinition
    {
        public Dream(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaAreaProps.WATER_COLOR, new Color(0, 0.48f, 0.89f, 1));
            SetProperty(VanillaAreaProps.WATER_COLOR_CENSORED, new Color(0, 0.48f, 0.89f, 1));
        }
        public override void Setup(LevelEngine level)
        {
            base.Setup(level);
            level.Spawn(VanillaEffectID.miner, new Vector3(600, 0, 40), null);
            if (Global.Game.IsUnlocked(VanillaUnlockID.dreamIsNightmare))
            {
                if (Global.HasBloodAndGore())
                {
                    level.SetAreaModelPreset(VanillaAreaModelPresets.Dream.nightmare);
                }
                else
                {
                    level.SetAreaModelPreset(VanillaAreaModelPresets.Dream.nightmareCensored);
                }
                level.AddBuff<NightmareLevelBuff>();
            }
        }
        public override void Update(LevelEngine level)
        {
            base.Update(level);
            var wave = GetPoolWave(level);
            wave = (wave + 0.01f) % 1;
            SetPoolWave(level, wave);
        }
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
            level.AddBuff<TaintedSunBuff>();
            level.PlaySound(VanillaSoundID.reverseVampire);
            level.PlaySound(VanillaSoundID.confuse);
        }
        public override float GetGroundY(LevelEngine level, float x, float z)
        {
            if (x > 500 && x < 820 && z > 120 && z < 440)
            {
                // 水中
                return -1 + Mathf.Sin(((x + z) * 0.01f + GetPoolWave(level)) * 4 * Mathf.PI);
            }
            return base.GetGroundY(level, x, z);
        }
        public static float GetPoolWave(LevelEngine level)
        {
            return level.GetProperty<float>(PROP_POOL_WAVE);
        }
        public static void SetPoolWave(LevelEngine level, float value)
        {
            level.SetProperty(PROP_POOL_WAVE, value);
        }
        public const string PROP_POOL_WAVE = "PoolWave";
    }
}