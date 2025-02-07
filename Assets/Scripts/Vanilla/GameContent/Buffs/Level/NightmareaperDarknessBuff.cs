using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Level.nightmareaperDarkness)]
    public class NightmareaperDarknessBuff : BuffDefinition
    {
        public NightmareaperDarknessBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaAreaProps.DARKNESS_VALUE, NumberOperator.Add, PROP_DARKNESS_ADDITION));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            var time = buff.GetProperty<int>(PROP_TIME);
            time++;
            buff.SetProperty(PROP_TIME, time);

            var darkness = Mathf.Min(time, timeout) / (float)FADE_TIME;

            buff.SetProperty(PROP_DARKNESS_ADDITION, Mathf.Clamp01(darkness));

            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public static void CancelDarkness(Buff buff)
        {
            buff.SetProperty(PROP_TIMEOUT, FADE_TIME);
        }
        public static readonly VanillaBuffPropertyMeta PROP_DARKNESS_ADDITION = new VanillaBuffPropertyMeta("DarknessAddition");
        public static readonly VanillaBuffPropertyMeta PROP_TIME = new VanillaBuffPropertyMeta("Time");
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
        public const float MAX_DARKNESS = 1;
        public const int FADE_TIME = 30;
    }
}
