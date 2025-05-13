using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.armor)]
    public static class VanillaArmorProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta NO_DISCARD = Get("noDiscard");
        public static bool HasNoDiscard(this Armor armor)
        {
            return armor.GetProperty<bool>(NO_DISCARD);
        }
        public static readonly PropertyMeta HIT_SOUND = Get("hitSound");
        public static NamespaceID GetHitSound(this Armor armor)
        {
            return armor.GetProperty<NamespaceID>(HIT_SOUND);
        }
        public static readonly PropertyMeta DEATH_SOUND = Get("deathSound");
        public static NamespaceID GetDeathSound(this Armor armor)
        {
            return armor.GetProperty<NamespaceID>(DEATH_SOUND);
        }
    }
}
