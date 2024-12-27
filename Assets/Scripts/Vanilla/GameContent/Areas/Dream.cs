using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Carts;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Grids;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using Tools;
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
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
            level.AddBuff<TaintedSunBuff>();
            level.PlaySound(VanillaSoundID.reverseVampire);
            level.PlaySound(VanillaSoundID.confuse);
        }
        public override float GetGroundY(float x, float z)
        {
            if (x > 500 && x < 820 && z > 120 && z < 440)
            {
                // 水中
                return -10;
            }
            return base.GetGroundY(x, z);
        }
    }
}