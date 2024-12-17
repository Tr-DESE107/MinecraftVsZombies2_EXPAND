using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Bosses
{
    public static class VanillaBossNames
    {
        public const string frankenstein = "frankenstein";
    }
    public static class VanillaBossID
    {
        public static readonly NamespaceID frankenstein = Get(VanillaBossNames.frankenstein);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
