using MVZ2.GameContent.Carts;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.nightmareLevel)]
    public class NightmareLevelBuff : BuffDefinition
    {
        public NightmareLevelBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(EngineAreaProps.CART_REFERENCE, VanillaCartID.nyaightmare));
            AddModifier(new NamespaceIDModifier(VanillaStageProps.CLEAR_SOUND, VanillaSoundID.agnoy));
            AddModifier(new ColorModifier(VanillaAreaProps.WATER_COLOR, BlendOperator.One, BlendOperator.Zero, new Color(0.89f, 0, 0, 1)));
            AddModifier(new ColorModifier(VanillaAreaProps.WATER_COLOR_CENSORED, BlendOperator.One, BlendOperator.Zero, new Color(0, 0, 0.5f, 1)));
        }
    }
}
