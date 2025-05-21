using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Armors
{
    [BuffDefinition(VanillaBuffNames.littleZombieArmor)]
    public class LittleZombieArmorBuff : BuffDefinition
    {
        public LittleZombieArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineArmorProps.MAX_HEALTH, NumberOperator.Multiply, 0.25f));
        }
    }
}
