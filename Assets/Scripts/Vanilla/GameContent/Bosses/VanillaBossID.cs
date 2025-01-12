using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Bosses
{
    public static class VanillaBossNames
    {
        public const string frankenstein = "frankenstein";
        public const string slenderman = "slenderman";
        public const string nightmareaper = "nightmareaper";
    }
    public static class VanillaBossID
    {
        public static readonly NamespaceID frankenstein = Get(VanillaBossNames.frankenstein);
        public static readonly NamespaceID slenderman = Get(VanillaBossNames.slenderman);
        public static readonly NamespaceID nightmareaper = Get(VanillaBossNames.nightmareaper);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
