using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ContraptionID
    {
        public static readonly NamespaceID lilyPad = Get("lilyPad");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
