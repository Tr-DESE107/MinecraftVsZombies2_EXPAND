using PVZEngine;

namespace MVZ2Logic
{
    public static class BuiltinUnlockID
    {
        public static readonly NamespaceID almanac = Get("almanac");
        public static readonly NamespaceID store = Get("store");
        public static readonly NamespaceID trigger = Get("trigger");
        public static readonly NamespaceID starshard = Get("starshard");
        private static NamespaceID Get(string path)
        {
            return new NamespaceID(Builtin.spaceName, path);
        }
    }
}
