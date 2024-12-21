using MVZ2.GameContent.Models;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Models;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.dreamKeyShield)]
    public class DreamKeyShieldBuff : BuffDefinition
    {
        public DreamKeyShieldBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.dreamKeyShield, VanillaModelID.dreamKeyShield);
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
        }
    }
}
