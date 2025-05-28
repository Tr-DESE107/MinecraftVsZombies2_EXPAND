﻿using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.iZombieSkeletonWarriorArmor)]
    public class IZombieSkeletonWarriorArmorBuff : BuffDefinition
    {
        public IZombieSkeletonWarriorArmorBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ArmorMaxHealthModifier(NumberOperator.Multiply, 3f));
        }
    }
}
