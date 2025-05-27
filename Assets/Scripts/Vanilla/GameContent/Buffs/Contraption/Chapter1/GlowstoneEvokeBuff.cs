using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.glowstoneEvoke)]
    public class GlowstoneEvokeBuff : BuffDefinition
    {
        public GlowstoneEvokeBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(VanillaEntityProps.LIGHT_RANGE, NumberOperator.Multiply, PROP_RANGE_MULTIPLIER));
            AddModifier(new ColorModifier(VanillaEntityProps.LIGHT_COLOR, BlendOperator.One, BlendOperator.One, PROP_COLOR_MULTIPLIER));
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
        public static readonly VanillaBuffPropertyMeta<FrameTimer> PROP_TIMER = new VanillaBuffPropertyMeta<FrameTimer>("Timer");
        public static readonly VanillaBuffPropertyMeta<Vector3> PROP_RANGE_MULTIPLIER = new VanillaBuffPropertyMeta<Vector3>("RangeMultiplier");
        public static readonly VanillaBuffPropertyMeta<Color> PROP_COLOR_MULTIPLIER = new VanillaBuffPropertyMeta<Color>("ColorMultiplier");
    }
}
