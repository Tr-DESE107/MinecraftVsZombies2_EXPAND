using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Talk
{
    public static class VanillaTalkNames
    {
        public const string tutorial = "tutorial";
        public const string starshardTutorial = "starshard_tutorial";
    }
    public static class VanillaTalkID
    {
        public static readonly NamespaceID tutorial = Get(VanillaTalkNames.tutorial);
        public static readonly NamespaceID starshardTutorial = Get(VanillaTalkNames.starshardTutorial);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
