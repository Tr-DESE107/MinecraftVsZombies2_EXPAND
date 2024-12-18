using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.ProgressBars
{
    public static class VanillaProgressBarID
    {
        public static readonly NamespaceID frankenstein = Get("frankenstein");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
