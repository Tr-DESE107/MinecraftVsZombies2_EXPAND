using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.damageColor)]
    public class DamageColorBuff : BuffDefinition
    {
        public DamageColorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, new Color(1, 0, 0, 0.5f)));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.Target.GetEntity();
            if (entity == null)
                return;
            if (!entity.IsDead)
            {
                buff.Remove();
            }
        }
    }
}
