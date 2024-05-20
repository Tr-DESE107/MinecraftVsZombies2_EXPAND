using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ShellID
    {
        public static readonly NamespaceID flesh = Get("flesh");
        public static readonly NamespaceID stone = Get("stone");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
