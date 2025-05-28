﻿using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.theGiantTransition)]
    public class TheGiantTransitionBuff : BuffDefinition
    {
        public TheGiantTransitionBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);

            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            var level = buff.Level;
            if (timeout >= STATE_1_TIMEOUT)
            {
                // 消除小神灵宇宙。
                foreach (var eye in level.FindEntities(VanillaEffectID.spiritUniverse))
                {
                    if (eye.Timeout <= 0)
                    {
                        eye.Timeout = 30;
                    }
                }
                // 音乐放缓。
                level.SetMusicVolume(Mathf.Clamp01(level.GetMusicVolume() - (1 / 30f)));
                if (timeout == SHAKE_TIMEOUT)
                {
                    level.PlaySound(VanillaSoundID.smallExplosion);
                    level.ShakeScreen(5, 0, 5);
                }
            }
            else if (timeout >= STATE_2_TIMEOUT)
            {
                if (timeout == BIG_SHAKE_TIMEOUT)
                {
                    level.PlaySound(VanillaSoundID.explosion);
                    level.PlaySound(VanillaSoundID.giantSpike);
                    level.ShakeScreen(15, 0, 30);
                }
                else if (timeout == ROAR_TIMEOUT)
                {
                    level.PlaySound(VanillaSoundID.giantRoar);
                    level.ShakeScreen(15, 0, 90);
                    level.Spawn(VanillaEffectID.amplifiedRoar, new Vector3(VanillaLevelExt.LEVEL_WIDTH, 0, level.GetLawnCenterZ()), null);
                }
                if (timeout == STATE_2_TIMEOUT)
                {
                    var giant = level.Spawn(VanillaBossID.theGiant, new Vector3(VanillaLevelExt.LEVEL_WIDTH, 0, level.GetLawnCenterZ()), null);
                    TheGiant.SetAppear(giant);
                    // 音乐。
                    level.PlayMusic(VanillaMusicID.mausoleumBoss);
                    level.SetMusicVolume(1);
                    level.SetSubtrackWeight(0);
                    level.SetProgressBarToBoss(VanillaProgressBarID.theGiant);
                    buff.Remove();
                }
            }
        }
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public const int MAX_TIMEOUT = STATE_1_TIMEOUT + 60;

        public const int STATE_1_TIMEOUT = SHAKE_TIMEOUT;
        public const int SHAKE_TIMEOUT = BIG_SHAKE_TIMEOUT + 30;

        public const int BIG_SHAKE_TIMEOUT = ROAR_TIMEOUT + 30;
        public const int ROAR_TIMEOUT = STATE_2_TIMEOUT + 100;
        public const int STATE_2_TIMEOUT = 0;
    }
}
