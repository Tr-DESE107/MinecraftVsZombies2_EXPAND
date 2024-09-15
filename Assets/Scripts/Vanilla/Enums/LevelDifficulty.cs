using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class LevelDifficulty
    {
        public static readonly NamespaceID easy = Get("easy");
        public static readonly NamespaceID normal = Get("normal");
        public static readonly NamespaceID hard = Get("hard");
        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}