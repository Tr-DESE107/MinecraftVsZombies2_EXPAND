using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Maps
{
    public static class VanillaMapPresetID
    {
        public static readonly NamespaceID nightmare = Get("nightmare");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
