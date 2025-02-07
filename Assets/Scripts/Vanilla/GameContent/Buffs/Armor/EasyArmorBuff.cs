using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Armors
{
    [BuffDefinition(VanillaBuffNames.easyArmor)]
    public class EasyArmorBuff : BuffDefinition
    {
        public EasyArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.MAX_HEALTH, NumberOperator.Multiply, 0.5f));
        }
    }
}
