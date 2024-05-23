using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class GridNames
    {
        public const string grass = "grass";
    }
    public static class GridID
    {
        public static readonly NamespaceID grass = Get(GridNames.grass);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
