using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Level.taintedSun)]
    public class TaintedSunBuff : BuffDefinition
    {
        public TaintedSunBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaAreaProps.DARKNESS_VALUE, NumberOperator.Add, PROP_DARKNESS_ADDITION));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_DARKNESS_ADDITION, 1);
            buff.SetProperty(PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            buff.SetProperty(PROP_DARKNESS_ADDITION, Mathf.Min(1, timeout / (float)FADE_MAX_TIMEOUT));
            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_DARKNESS_ADDITION = new VanillaBuffPropertyMeta("DarknessAddition");
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
        public const int MAX_TIMEOUT = 330;
        public const int FADE_MAX_TIMEOUT = 30;
    }
}
