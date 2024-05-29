using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2
{
    public static class SpritePaths
    {
        public static readonly NamespaceID pickaxe = Get("pickaxe");

        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, $"Textures/{name}");
        }
        public static NamespaceID GetStarshardIcon(NamespaceID areaID)
        {
            return new NamespaceID(areaID.spacename, $"Textures/starshard.{areaID.path}");
        }
    }
}
