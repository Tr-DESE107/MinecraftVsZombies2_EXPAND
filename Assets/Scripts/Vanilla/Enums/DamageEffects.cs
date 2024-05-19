using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class DamageEffects
    {
        public static readonly NamespaceID FIRE = Get("fire");
        public static readonly NamespaceID SLICE = Get("slice");

        public static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
