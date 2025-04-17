using MVZ2.GameContent.Areas;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.nightmareCleared)]
    public class NightmareClearedBuff : BuffDefinition
    {
        public NightmareClearedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(LogicLevelProps.SCREEN_COVER, PROP_SCREEN_COVER));
            AddModifier(new BooleanModifier(LogicLevelProps.PAUSE_DISABLED, true));
            AddModifier(new BooleanModifier(VanillaStageProps.AUTO_COLLECT_ALL, true));
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

            if (timeout > LIGHT_TIMEOUT)
            {
                // 屏幕逐渐变黑
                var cover = buff.GetProperty<Color>(PROP_SCREEN_COVER);
                var t = (timeout - LIGHT_TIMEOUT) / (float)(MAX_TIMEOUT - LIGHT_TIMEOUT);
                t = t * 2 - 1;
                cover = Color.Lerp(Color.black, Color.clear, t);
                buff.SetProperty(PROP_SCREEN_COVER, cover);
            }
            else if (timeout > LIGHT_FADE_TIMEOUT)
            {
                if (timeout == LIGHT_TIMEOUT)
                {
                    Dream.SetToDream(level);
                }
                // 屏幕变白
                var cover = buff.GetProperty<Color>(PROP_SCREEN_COVER);
                cover = Color.Lerp(Color.white, Color.black, (timeout - LIGHT_FADE_TIMEOUT) / (float)(LIGHT_TIMEOUT - LIGHT_FADE_TIMEOUT));
                buff.SetProperty(PROP_SCREEN_COVER, cover);
            }
            else
            {
                if (timeout == LIGHT_FADE_TIMEOUT)
                {
                    Global.Game.Relock(VanillaUnlockID.dreamIsNightmare);
                    level.Clear();
                }
                // 屏幕恢复
                var cover = buff.GetProperty<Color>(PROP_SCREEN_COVER);
                cover = Color.Lerp(new Color(1, 1, 1, 0), Color.white, timeout / (float)(LIGHT_FADE_TIMEOUT));
                buff.SetProperty(PROP_SCREEN_COVER, cover);
                if (timeout <= 0)
                {
                    buff.Remove();
                }
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
        public static readonly VanillaBuffPropertyMeta PROP_SCREEN_COVER = new VanillaBuffPropertyMeta("ScreenCover");
        public const float BLACK_SCREEN_SPEED = 1 / 180f;
        public const int MAX_TIMEOUT = LIGHT_TIMEOUT + 120;
        public const int LIGHT_TIMEOUT = LIGHT_FADE_TIMEOUT + 30;
        public const int LIGHT_FADE_TIMEOUT = 60;
    }
}
