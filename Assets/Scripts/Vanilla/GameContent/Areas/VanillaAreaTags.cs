using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Areas
{
    public static class VanillaAreaTags
    {
        public static readonly NamespaceID day = Get("day");
        public static readonly NamespaceID night = Get("night");
        public static readonly NamespaceID noWater = Get("no_water");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
