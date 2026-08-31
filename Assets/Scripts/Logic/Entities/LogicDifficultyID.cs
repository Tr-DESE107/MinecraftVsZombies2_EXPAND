#nullable enable

using PVZEngine;

namespace MVZ2Logic.Difficulties
{
    public static class LogicDifficultyNames
    {
        public const string easy = "easy";
        public const string normal = "normal";
        public const string hard = "hard";
        public const string lunatic = "lunatic";
    }
    public static class LogicDifficultyID
    {
        public static readonly NamespaceID easy = Get(LogicDifficultyNames.easy);
        public static readonly NamespaceID normal = Get(LogicDifficultyNames.normal);
        public static readonly NamespaceID hard = Get(LogicDifficultyNames.hard);
        public static readonly NamespaceID lunatic = Get(LogicDifficultyNames.lunatic);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Global.BuiltinNamespace, name);
        }
    }
}
