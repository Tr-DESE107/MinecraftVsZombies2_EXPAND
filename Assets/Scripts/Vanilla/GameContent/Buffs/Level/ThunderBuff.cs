using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Level.thunder)]
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
        public const string PROP_DARKNESS_MULTIPLIER = "DarknessMultiplier";
        public const string PROP_TIMEOUT = "Timeout";
        public const int MAX_TIMEOUT = 30;
    }
}
