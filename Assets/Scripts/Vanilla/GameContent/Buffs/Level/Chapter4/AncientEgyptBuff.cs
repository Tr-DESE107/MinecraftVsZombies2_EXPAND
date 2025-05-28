using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.ancientEgypt)]
    public class AncientEgyptBuff : BuffDefinition
    {
        public AncientEgyptBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(LogicLevelProps.MUSIC_LOW_QUALITY, true));
            AddModifier(new BooleanModifier(LogicLevelProps.GRAPHICS_DOWNGRADE, true));
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
            }
            timer.Run();
            if (timer.Expired)
            {
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("Timer");
        public const int MAX_TIMEOUT = 900;
    }
}
