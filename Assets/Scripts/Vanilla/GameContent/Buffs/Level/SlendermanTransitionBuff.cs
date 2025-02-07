using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.ProgressBars;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.slendermanTransition)]
    public class SlendermanTransitionBuff : BuffDefinition
    {
        public SlendermanTransitionBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaAreaProps.NIGHT_VALUE, NumberOperator.Add, PROP_NIGHT_ADDITION));
            AddModifier(new BooleanModifier(LogicLevelProps.PAUSE_DISABLED, true));
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
            if (timeout > CREATE_DARKNESS_TIMEOUT)
            {
                // 让眼睛闭眼。
                foreach (var eye in level.FindEntities(VanillaEffectID.nightmareWatchingEye))
                {
                    if (eye.Timeout <= 0)
                    {
                        eye.Timeout = 30;
                    }
                }
                // 音乐放缓。
                level.SetMusicVolume(Mathf.Clamp01(level.GetMusicVolume() - (1 / 30f)));
            }
            if (timeout == CREATE_DARKNESS_TIMEOUT)
            {
                level.Spawn(VanillaEffectID.nightmareDarkness, Vector3.zero, null);
                // 音乐。
                level.PlayMusic(VanillaMusicID.nightmareBoss);
                level.SetMusicVolume(1);
            }
            else if (timeout <= 0)
            {
                Vector3 pos = new Vector3(level.GetEntityColumnX(4), 0, level.GetEntityLaneZ(2));
                var boss = level.Spawn(VanillaBossID.slenderman, pos, null);
                boss.Velocity = Vector3.up * 5;
                boss.PlaySound(VanillaSoundID.splashBig);
                boss.PlaySound(VanillaSoundID.glassBreakBig);
                level.ShakeScreen(30, 0, 30);
                boss.Spawn(VanillaEffectID.nightmareaperSplash, pos);

                level.SetProgressBarToBoss(VanillaProgressBarID.nightmare);

                buff.Remove();
            }

            var addition = buff.GetProperty<float>(PROP_NIGHT_ADDITION);
            var nightSpeed = NIGHT_SPEED;
            if (timeout < CREATE_DARKNESS_TIMEOUT)
            {
                nightSpeed = -NIGHT_SPEED;
            }
            addition = Mathf.Clamp01(addition + nightSpeed);
            buff.SetProperty(PROP_NIGHT_ADDITION, addition);
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
        public static readonly VanillaBuffPropertyMeta PROP_NIGHT_ADDITION = new VanillaBuffPropertyMeta("NightAddition");
        public const float NIGHT_SPEED = 0.07f;
        public const int MAX_TIMEOUT = 270;
        public const int CREATE_DARKNESS_TIMEOUT = MAX_TIMEOUT - 90;
    }
}
