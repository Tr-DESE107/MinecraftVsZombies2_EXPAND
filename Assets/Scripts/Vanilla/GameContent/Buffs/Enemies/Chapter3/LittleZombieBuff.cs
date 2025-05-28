﻿using MVZ2.GameContent.Buffs.Armors;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.littleZombie)]
    public class LittleZombieBuff : BuffDefinition
    {
        public LittleZombieBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.SCALE, NumberOperator.Multiply, new Vector3(0.5f, 0.5f, 0.5f)));
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, new Vector3(0.5f, 0.5f, 0.5f)));
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHADOW_SCALE, NumberOperator.Multiply, new Vector3(0.5f, 0.5f, 0.5f)));
            AddModifier(new MaxHealthModifier(NumberOperator.Multiply, 0.25f));
            AddModifier(new FloatModifier(VanillaEntityProps.CRY_PITCH, NumberOperator.Multiply, 2));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var armor = entity.GetMainArmor();
            if (armor == null)
                return;
            if (armor.HasBuff<LittleZombieArmorBuff>())
                return;
            armor.AddBuff<LittleZombieArmorBuff>();
        }
    }
}
