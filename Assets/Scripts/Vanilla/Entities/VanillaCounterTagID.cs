using PVZEngine;

namespace MVZ2.Vanilla.Almanacs
{
    public class VanillaCounterTagID
    {
        public static readonly NamespaceID lowEnemy = Get("low_enemy");
        public static readonly NamespaceID flyingEnemy = Get("flying_enemy");

        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
