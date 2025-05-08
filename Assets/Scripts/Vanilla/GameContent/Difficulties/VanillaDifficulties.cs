using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Difficulties
{
    public static class VanillaDifficulties
    {
        public static readonly NamespaceID easy = Get("easy");
        public static readonly NamespaceID normal = Get("normal");
        public static readonly NamespaceID hard = Get("hard");
        public static readonly NamespaceID lunatic = Get("lunatic");
        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}