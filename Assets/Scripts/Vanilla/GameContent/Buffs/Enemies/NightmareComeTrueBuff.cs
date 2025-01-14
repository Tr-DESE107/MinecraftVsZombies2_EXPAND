using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.nightmareComeTrue)]
    public class NightmareComeTrueBuff : BuffDefinition
    {
        public NightmareComeTrueBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.ATTACK_SPEED, NumberOperator.Multiply, 2f));
            AddModifier(new FloatModifier(VanillaEntityProps.DAMAGE, NumberOperator.Multiply, 2f));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            entity.Health = entity.GetMaxHealth();
        }
    }
}
