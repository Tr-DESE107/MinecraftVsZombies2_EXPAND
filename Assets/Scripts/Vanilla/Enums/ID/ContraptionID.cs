using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ContraptionID
    {
        public static readonly NamespaceID dispenser = Get("dispenser");
        public static readonly NamespaceID furnace = Get("furnace");
        public static readonly NamespaceID lilyPad = Get("lily_pad");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
