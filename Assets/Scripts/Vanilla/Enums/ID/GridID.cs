using PVZEngine;

namespace MVZ2.GameContent
{
    public static class GridID
    {
        public static readonly NamespaceID grass = Get("grass");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
