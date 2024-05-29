using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2
{
    public static class SpritePaths
    {
        public static readonly NamespaceID pickaxe = Get("pickaxe");

        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, $"{name}");
        }
        public static NamespaceID GetStarshardIcon(NamespaceID areaID)
        {
            return new NamespaceID(areaID.spacename, $"starshard.{areaID.path}");
        }
    }
}
