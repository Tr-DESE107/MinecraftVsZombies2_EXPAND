using MVZ2.GameContent.Buffs.Armors;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.littleZombie)]
    public class LittleZombieBuff : BuffDefinition
    {
        public LittleZombieBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.SCALE, NumberOperator.Multiply, new Vector3(0.5f, 0.5f, 0.5f)));
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, new Vector3(0.5f, 0.5f, 0.5f)));
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHADOW_SCALE, NumberOperator.Multiply, new Vector3(0.5f, 0.5f, 0.5f)));
            AddModifier(new FloatModifier(EngineEntityProps.MAX_HEALTH, NumberOperator.Multiply, 0.25f));
            AddModifier(new FloatModifier(VanillaEntityProps.CRY_PITCH, NumberOperator.Multiply, 2));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            if (entity.EquipedArmor == null)
                return;
            if (entity.EquipedArmor.HasBuff<LittleZombieArmorBuff>())
                return;
            entity.EquipedArmor.AddBuff<LittleZombieArmorBuff>();
            entity.EquipedArmor.Health = entity.EquipedArmor.GetMaxHealth();
        }
    }
}
