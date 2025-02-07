using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Level.thunder)]
    public class ThunderBuff : BuffDefinition
    {
        public ThunderBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaAreaProps.NIGHT_VALUE, NumberOperator.Multiply, PROP_DARKNESS_MULTIPLIER));
            AddModifier(new FloatModifier(VanillaAreaProps.DARKNESS_VALUE, NumberOperator.Multiply, PROP_DARKNESS_MULTIPLIER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_DARKNESS_MULTIPLIER, 0);
            buff.SetProperty(PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            buff.SetProperty(PROP_DARKNESS_MULTIPLIER, 1 - timeout / (float)MAX_TIMEOUT);
            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_DARKNESS_MULTIPLIER = new VanillaBuffPropertyMeta("DarknessMultiplier");
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
        public const int MAX_TIMEOUT = 30;
    }
}
