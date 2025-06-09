using MVZ2.GameContent.Buffs.Armors;
using PVZEngine.Armors;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaArmorExt
    {
        public static void DamageBlink(this Armor armor)
        {
            if (armor != null && !armor.HasBuff<ArmorDamageColorBuff>())
                armor.AddBuff<ArmorDamageColorBuff>();
        }
        public static int GetHealthState(this Armor entity, int stateCount)
        {
            return GetHealthState(entity.Health, entity.GetMaxHealth(), stateCount);
        }
        public static int GetHealthState(float health, float maxHealth, int stateCount)
        {
            float stateHP = maxHealth / stateCount;
            return stateCount - Mathf.CeilToInt(health / stateHP);
        }
        public static void SetModelHealthStateByCount(this Armor armor, int count)
        {
            armor.SetModelHealthState(armor.GetHealthState(count));
        }
        public static void SetModelHealthState(this Armor armor, int state)
        {
            armor.SetModelProperty("HealthState", state);
        }
    }
}
