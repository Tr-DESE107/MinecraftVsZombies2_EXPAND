using MVZ2.GameContent.Fragments;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.Contraption.hellfireCursed)]
    public class HellfireCursedBuff : BuffDefinition
    {
        public HellfireCursedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(VanillaContraptionProps.FRAGMENT_ID, VanillaFragmentID.hellfireCursed));
            AddModifier(ColorModifier.Override(VanillaEntityProps.LIGHT_COLOR, new Color(0, 1, 0)));
        }
    }
}
