using MVZ2.GameContent.Buffs.Armors;
using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaArmorExt
    {
        public static void DamageBlink(this Armor armor)
        {
            if (armor != null && !armor.HasBuff<ArmorDamageColorBuff>())
                armor.AddBuff<ArmorDamageColorBuff>();
        }
        public static void SetModelDamagePercent(this Armor armor)
        {
            armor.SetModelDamagePercent(armor.Health, armor.GetMaxHealth());
        }
        public static void SetModelDamagePercent(this Armor armor, float health, float maxHealth)
        {
            armor.SetModelDamagePercent(1 - health / maxHealth);
        }
        public static void SetModelDamagePercent(this Armor armor, float percent)
        {
            armor.SetModelProperty("DamagePercent", percent);
        }
    }
}
