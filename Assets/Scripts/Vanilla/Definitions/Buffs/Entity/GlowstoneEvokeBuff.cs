using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.glowstoneEvoke)]
    public class GlowstoneEvokeBuff : BuffDefinition
    {
        public GlowstoneEvokeBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(BuiltinEntityProps.LIGHT_RANGE, NumberOperator.Multiply, PROP_RANGE_MULTIPLIER));
            AddModifier(new ColorModifier(BuiltinEntityProps.LIGHT_COLOR, BlendOperator.One, BlendOperator.One, PROP_COLOR_MULTIPLIER));
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
            buff.SetProperty(PROP_RANGE_MULTIPLIER, Vector3.one * percentage * 10);
            buff.SetProperty(PROP_COLOR_MULTIPLIER, new Color(percentage, percentage, percentage, percentage));
        }
        public const string PROP_TIMER = "Timer";
        public const string PROP_RANGE_MULTIPLIER = "RangeMultiplier";
        public const string PROP_COLOR_MULTIPLIER = "ColorMultiplier";
    }
}
