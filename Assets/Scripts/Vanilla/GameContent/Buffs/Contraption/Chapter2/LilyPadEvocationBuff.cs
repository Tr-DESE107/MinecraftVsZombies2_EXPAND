using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.Contraption.lilyPadEvocation)]
    public class LilyPadEvocationBuff : BuffDefinition
    {
        public LilyPadEvocationBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.SIZE, NumberOperator.Multiply, PROP_SIZE_MULTIPLIER));
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_SIZE_MULTIPLIER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(15));
            UpdateMultipliers(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateMultipliers(buff);

            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            timer.Run();
            if (timer.Expired)
            {
                buff.Remove();
            }
        }
        private void UpdateMultipliers(Buff buff)
        {
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            var percentage = timer.Frame / (float)timer.MaxFrame;
            buff.SetProperty(PROP_SIZE_MULTIPLIER, Vector3.one * (1 - percentage));
        }
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("Timer");
        public static readonly VanillaBuffPropertyMeta<Vector3> PROP_SIZE_MULTIPLIER = new VanillaBuffPropertyMeta<Vector3>("SizeMultiplier");
    }
}
