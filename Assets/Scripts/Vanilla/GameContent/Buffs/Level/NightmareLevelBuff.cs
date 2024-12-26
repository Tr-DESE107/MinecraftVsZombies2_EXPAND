using MVZ2.GameContent.Carts;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Level.nightmareLevel)]
    public class NightmareLevelBuff : BuffDefinition
    {
        public NightmareLevelBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(EngineAreaProps.CART_REFERENCE, VanillaCartID.nyaightmare));
            AddModifier(new NamespaceIDModifier(VanillaLevelProps.MUSIC_ID, VanillaMusicID.nightmareLevel));
            SetProperty(VanillaAreaProps.WATER_COLOR, new Color(0.89f, 0, 0, 1));
            SetProperty(VanillaAreaProps.WATER_COLOR_CENSORED, new Color(0, 0, 0.5f, 1));
        }
    }
}
