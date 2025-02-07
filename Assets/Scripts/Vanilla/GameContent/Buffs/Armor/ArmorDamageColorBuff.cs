using MVZ2.Vanilla.Properties;
using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Armors
{
    [BuffDefinition(VanillaBuffNames.armorDamageColor)]
    public class ArmorDamageColorBuff : BuffDefinition
    {
        public ArmorDamageColorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineArmorProps.COLOR_OFFSET, new Color(1, 0, 0, 0.5f)));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, 2);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout > 0)
                return;
            buff.Remove();
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
    }
}
