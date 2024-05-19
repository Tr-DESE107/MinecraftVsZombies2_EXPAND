using PVZEngine;

namespace MVZ2.GameContent
{
    public static class BuffID
    {
        public static readonly NamespaceID randomEnemySpeed = Get("randomEnemySpeed");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
