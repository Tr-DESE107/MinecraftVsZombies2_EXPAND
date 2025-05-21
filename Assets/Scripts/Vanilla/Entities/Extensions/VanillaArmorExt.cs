using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Armors;
using PVZEngine.Armors;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaArmorExt
    {
        public static void DamageBlink(this Armor armor)
        {
            if (armor != null && !armor.HasBuff<ArmorDamageColorBuff>())
                armor.AddBuff<ArmorDamageColorBuff>();
        }
    }
}
