using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;
using Tools;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.temporaryUpdateBeforeGame)]
    public class TemporaryUpdateBeforeGameBuff : BuffDefinition
    {
        public TemporaryUpdateBeforeGameBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.UPDATE_BEFORE_GAME, true));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(32));
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
            timer.Run();
            if (timer.Expired)
            {
                buff.Remove();
                return;
            }
        }
        public const string PROP_TIMER = "Timer";
    }
}
