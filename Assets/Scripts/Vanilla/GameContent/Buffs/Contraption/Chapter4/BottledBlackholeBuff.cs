using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.Contraption.bottledBlackholeDamage)]
    public class BottledBlackholeDamageBuff : BuffDefinition
    {
        public BottledBlackholeDamageBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.DAMAGE, NumberOperator.AddMultiple, PROP_DAMAGE_MULTIPLIER));
        }
        public static void SetDamageMultiplier(Buff buff, float value)
        {
            buff.SetProperty(PROP_DAMAGE_MULTIPLIER, value);
        }
        public static readonly VanillaBuffPropertyMeta<float> PROP_DAMAGE_MULTIPLIER = new VanillaBuffPropertyMeta<float>("damageMultiplier");
    }
}
