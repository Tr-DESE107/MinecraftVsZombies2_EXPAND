using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Armors
{
    [Definition(VanillaBuffNames.littleZombieArmor)]
    public class LittleZombieArmorBuff : BuffDefinition
    {
        public LittleZombieArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.MAX_HEALTH, NumberOperator.Multiply, 0.25f));
        }
    }
}
