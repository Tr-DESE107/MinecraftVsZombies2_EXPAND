﻿using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.witherCleared)]
    public class WitherClearedBuff : BuffDefinition
    {
        public WitherClearedBuff(string nsp, string name) : base(nsp, name)
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

            if (timeout > FADEOUT_TIMEOUT)
            {
                // 屏幕变白
                var t = 1 - (timeout - FADEOUT_TIMEOUT) / (float)(MAX_TIMEOUT - FADEOUT_TIMEOUT);
                t = t * 2;
                var cover = Color.Lerp(Color.clear, Color.white, t);
                buff.SetProperty(PROP_SCREEN_COVER, cover);
            }
            else if (timeout > 0)
            {
                // 屏幕淡出
                var t = 1 - timeout / (float)(FADEOUT_TIMEOUT);
                var cover = Color.Lerp(Color.white, Color.clear, t);
                buff.SetProperty(PROP_SCREEN_COVER, cover);
            }
            else
            {
                level.Clear();
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public static readonly VanillaBuffPropertyMeta<Color> PROP_SCREEN_COVER = new VanillaBuffPropertyMeta<Color>("ScreenCover");
        public const float BLACK_SCREEN_SPEED = 1 / 180f;
        public const int MAX_TIMEOUT = FADEOUT_TIMEOUT + 30;
        public const int FADEOUT_TIMEOUT = 60;
    }
}
