using PVZEngine;

namespace MVZ2.GameContent
{
    public static class AreaID
    {
        public static readonly NamespaceID day = Get("day");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
