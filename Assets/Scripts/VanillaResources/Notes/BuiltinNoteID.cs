using PVZEngine;

namespace MVZ2Logic.Notes
{
    public static class BuiltinNoteNames
    {
        public const string help = "help";
    }
    public static class BuiltinNoteID
    {
        public static readonly NamespaceID help = Get(BuiltinNoteNames.help);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
