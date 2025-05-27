using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Level.frankensteinStage)]
    public class FrankensteinStageBuff : BuffDefinition
    {
        public FrankensteinStageBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(ColorModifier.Multiply(VanillaAreaProps.GLOBAL_LIGHT, PROP_LIGHT_MULTIPLIER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_LIGHT_MULTIPLIER, Color.white);
            buff.SetProperty(PROP_THUNDER_TIMEOUT, MAX_THUNDER_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_THUNDER_TIMEOUT);
            timeout--;
            if (timeout <= 0)
            {
                timeout = MAX_THUNDER_TIMEOUT;
                buff.Level.Thunder();
            }
            buff.SetProperty(PROP_THUNDER_TIMEOUT, timeout);


            var time = buff.GetProperty<int>(PROP_TIME);
            if (time < MAX_TIME)
            {
                time++;
                buff.SetProperty(PROP_TIME, time);
            }
            var colorComp = 1 - (time / (float)MAX_TIME) * 0.5f;
            buff.SetProperty(PROP_LIGHT_MULTIPLIER, new Color(colorComp, colorComp, colorComp, 1));
        }
        public static readonly VanillaBuffPropertyMeta<Color> PROP_LIGHT_MULTIPLIER = new VanillaBuffPropertyMeta<Color>("LightMultiplier");
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIME = new VanillaBuffPropertyMeta<int>("Time");
        public static readonly VanillaBuffPropertyMeta<int> PROP_THUNDER_TIMEOUT = new VanillaBuffPropertyMeta<int>("ThunderTimeout");
        public const int MAX_THUNDER_TIMEOUT = 150;
        public const int MAX_TIME = 30;
    }
}
