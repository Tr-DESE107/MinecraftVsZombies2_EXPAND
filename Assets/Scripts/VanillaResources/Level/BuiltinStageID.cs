using PVZEngine;

namespace MVZ2Logic.Level
{
    public static class BuiltinStageID
    {
        public static readonly NamespaceID prologue = Get("prologue");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
