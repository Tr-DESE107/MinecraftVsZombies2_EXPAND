using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class AreaNames
    {
        public const string day = "day";
    }
    public static class AreaID
    {
        public static readonly NamespaceID day = Get(AreaNames.day);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
