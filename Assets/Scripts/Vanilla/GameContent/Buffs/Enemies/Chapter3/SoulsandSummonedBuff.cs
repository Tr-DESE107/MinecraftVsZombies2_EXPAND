using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.soulsandSummoned)]
    public class SoulsandSummonedBuff : BuffDefinition
    {
        public SoulsandSummonedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.SCALE, NumberOperator.Multiply, PROP_SCALE));
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_SCALE));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_SCALE, Vector3.zero);
            buff.SetProperty(PROP_TIMER, new FrameTimer(15));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            timer.Run();
            buff.SetProperty(PROP_SCALE, Vector3.one * (1 - timer.Frame / (float)timer.MaxFrame));
            if (timer.Expired)
            {
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_SCALE = new VanillaBuffPropertyMeta("Scale");
        public static readonly VanillaBuffPropertyMeta PROP_TIMER = new VanillaBuffPropertyMeta("Timer");
    }
}
