using MVZ2.GameContent.Carts;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Level.nightmareLevel)]
    public class NightmareLevelBuff : BuffDefinition
    {
        public NightmareLevelBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(EngineAreaProps.CART_REFERENCE, VanillaCartID.nyaightmare));
            AddModifier(new NamespaceIDModifier(VanillaLevelProps.MUSIC_ID, VanillaMusicID.nightmareLevel));
        }
    }
}
