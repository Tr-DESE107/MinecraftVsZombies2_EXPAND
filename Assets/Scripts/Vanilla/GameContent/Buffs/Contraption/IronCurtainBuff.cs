using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.ironCurtain)]
    public class IronCurtainBuff : BuffDefinition
    {
        public IronCurtainBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_TINT));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            var comp = Mathf.Cos(Mathf.Deg2Rad * timeout * 24) * 0.25f + 0.5f;
            var tint = new Color(comp, comp, comp, 1);
            buff.SetProperty(PROP_TINT, tint);

            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public const string PROP_TIMEOUT = "Timeout";
        public const string PROP_TINT = "Tint";
        public const int MAX_TIMEOUT = 150;
    }
}
