using PVZEngine;

namespace MVZ2.GameContent
{
    public static class EnemyID
    {
        public static readonly NamespaceID zombie = Get("zombie");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
