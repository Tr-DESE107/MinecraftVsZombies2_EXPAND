using PVZEngine;

namespace MVZ2Logic.Talk
{
    public static class TalkNames
    {
        public const string tutorial = "tutorial";
        public const string starshardTutorial = "starshard_tutorial";
    }
    public static class TalkID
    {
        public static readonly NamespaceID tutorial = Get(TalkNames.tutorial);
        public static readonly NamespaceID starshardTutorial = Get(TalkNames.starshardTutorial);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
