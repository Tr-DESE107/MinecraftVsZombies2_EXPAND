using PVZEngine;

namespace MVZ2.GameContent
{
    public static class PickupID
    {
        public static readonly NamespaceID redstone = Get("redstone");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
