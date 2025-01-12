using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.ProgressBars
{
    public static class VanillaProgressBarID
    {
        public static readonly NamespaceID frankenstein = Get("frankenstein");
        public static readonly NamespaceID nightmare = Get("nightmare");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
