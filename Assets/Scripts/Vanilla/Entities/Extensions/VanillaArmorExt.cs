using MVZ2.GameContent.Buffs;
using PVZEngine.Armors;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaArmorExt
    {
        public static void DamageBlink(this Armor armor)
        {
            if (armor != null && !armor.HasBuff<DamageColorBuff>())
                armor.AddBuff<DamageColorBuff>();
        }
    }
}
