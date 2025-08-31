using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.Contraption.carryingOther)]
    public class CarryingOtherBuff : BuffDefinition
    {
        public CarryingOtherBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.INVISIBLE, true));
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
            AddModifier(new BooleanModifier(VanillaEntityProps.NO_HELD_TARGET, true));
        }
    }
}
