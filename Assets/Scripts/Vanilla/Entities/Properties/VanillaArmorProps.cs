﻿using PVZEngine;
using PVZEngine.Armors;

namespace MVZ2.Vanilla.Entities
{
    [PropertyRegistryRegion(PropertyRegions.armor)]
    public static class VanillaArmorProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<bool> NO_DISCARD = Get<bool>("noDiscard");
        public static bool HasNoDiscard(this Armor armor)
        {
            return armor.GetProperty<bool>(NO_DISCARD);
        }
        public static readonly PropertyMeta<NamespaceID> HIT_SOUND = Get<NamespaceID>("hitSound");
        public static NamespaceID GetHitSound(this Armor armor)
        {
            return armor.GetProperty<NamespaceID>(HIT_SOUND);
        }
        public static readonly PropertyMeta<NamespaceID> DEATH_SOUND = Get<NamespaceID>("deathSound");
        public static NamespaceID GetDeathSound(this Armor armor)
        {
            return armor.GetProperty<NamespaceID>(DEATH_SOUND);
        }
    }
}

