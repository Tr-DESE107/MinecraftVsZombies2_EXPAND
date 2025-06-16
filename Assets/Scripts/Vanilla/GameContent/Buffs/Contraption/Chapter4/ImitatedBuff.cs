using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Modifiers;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.imitated)]
    public class ImitatedBuff : BuffDefinition
    {
        public ImitatedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.GRAYSCALE, true));
            AddModifier(ColorModifier.Override(VanillaEntityProps.LIGHT_COLOR, Color.white, priority: VanillaModifierPriorities.FORCE));
        }
    }
}
