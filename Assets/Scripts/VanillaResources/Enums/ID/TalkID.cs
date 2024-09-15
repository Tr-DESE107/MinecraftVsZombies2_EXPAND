using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class TalkNames
    {
        public const string tutorial = "tutorial";
    }
    public static class TalkID
    {
        public static readonly NamespaceID tutorial = Get(TalkNames.tutorial);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
