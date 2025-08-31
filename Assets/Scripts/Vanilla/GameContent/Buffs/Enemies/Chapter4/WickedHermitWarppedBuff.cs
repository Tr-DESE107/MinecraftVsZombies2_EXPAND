using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.Enemy.wickedHermitWarpped)]
    public class WickedHermitWarppedBuff : BuffDefinition
    {
        public WickedHermitWarppedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.FLIP_X, true));
        }
    }
}
