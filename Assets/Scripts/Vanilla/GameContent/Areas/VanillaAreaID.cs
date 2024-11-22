using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Areas
{
    public static class VanillaAreaNames
    {
        public const string day = "day";
        public const string halloween = "halloween";
    }
    public static class VanillaAreaID
    {
        public static readonly NamespaceID day = Get(VanillaAreaNames.day);
        public static readonly NamespaceID halloween = Get(VanillaAreaNames.halloween);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
