using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class VanillaObstacleNames
    {
        public const string gargoyleStatue = "gargoyle_statue";
    }
    public static class VanillaObstacleID
    {
        public static readonly NamespaceID gargoyleStatue = Get(VanillaObstacleNames.gargoyleStatue);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
