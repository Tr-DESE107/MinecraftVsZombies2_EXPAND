using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.miracleMalletReplicaDamage)]
    public class MiracleMalletReplicaDamageBuff : BuffDefinition
    {
        public MiracleMalletReplicaDamageBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.DAMAGE, NumberOperator.AddMultiplie, 0.15f));
        }
    }
}
