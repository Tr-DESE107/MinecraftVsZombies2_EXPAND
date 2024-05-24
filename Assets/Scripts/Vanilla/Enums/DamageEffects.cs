using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class DamageEffects
    {
        public static readonly NamespaceID FIRE = Get("fire");
        public static readonly NamespaceID SLICE = Get("slice");
        public static readonly NamespaceID PUNCH = Get("punch");
        public static readonly NamespaceID MUTE = Get("mute");
        public static readonly NamespaceID REMOVE_ON_DEATH = Get("remove_on_death");

        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
