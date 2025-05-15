using MVZ2.GameContent.Areas;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.flashWhite)]
    public class FlashWhiteBuff : BuffDefinition
    {
        public FlashWhiteBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(LogicLevelProps.SCREEN_COVER, PROP_SCREEN_COVER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIMEOUT));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            if (timer == null)
            {
                buff.Remove();
                return;
            }
            var level = buff.Level;

            timer.Run();

            // 屏幕恢复
            var cover = buff.GetProperty<Color>(PROP_SCREEN_COVER);
            cover = Color.Lerp(Color.white, new Color(1, 1, 1, 0), timer.GetPassedPercentage());
            buff.SetProperty(PROP_SCREEN_COVER, cover);
            if (timer.Expired)
            {
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMER = new VanillaBuffPropertyMeta("Timer");
        public static readonly VanillaBuffPropertyMeta PROP_SCREEN_COVER = new VanillaBuffPropertyMeta("ScreenCover");
        public const int MAX_TIMEOUT = 30;
    }
}
