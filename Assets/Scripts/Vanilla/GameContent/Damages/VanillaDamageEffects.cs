using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Damages
{
    public static class VanillaDamageEffects
    {
        public readonly static NamespaceID DAMAGE_BOTH_ARMOR_AND_BODY = Get("damage_both_armor_and_body");
        public readonly static NamespaceID DAMAGE_BODY_AFTER_ARMOR_BROKEN = Get("damage_body_after_armor_broken");
        public readonly static NamespaceID IGNORE_ARMOR = Get("ignore_armor");
        public readonly static NamespaceID FALL_DAMAGE = Get("fall_damage");
        public static readonly NamespaceID FIRE = Get("fire");
        public static readonly NamespaceID DROWN = Get("drown");
        public static readonly NamespaceID SLICE = Get("slice");
        public static readonly NamespaceID PUNCH = Get("punch");
        public static readonly NamespaceID MUTE = Get("mute");
        public static readonly NamespaceID REMOVE_ON_DEATH = Get("remove_on_death");
        public static readonly NamespaceID SACRIFICE = Get("sacrifice");
        public static readonly NamespaceID EXPLOSION = Get("explosion");
        public static readonly NamespaceID SELF_DAMAGE = Get("self_damage");
        public static readonly NamespaceID WHACK = Get("whack");
        public static readonly NamespaceID LIGHTNING = Get("lightning");
        public static readonly NamespaceID TINY = Get("tiny");
        public static readonly NamespaceID GROUND_SPIKES = Get("ground_spikes");
        public static readonly NamespaceID GOLD = Get("gold");
        public static readonly NamespaceID NO_NEUTRALIZE = Get("no_neutralize");

        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
