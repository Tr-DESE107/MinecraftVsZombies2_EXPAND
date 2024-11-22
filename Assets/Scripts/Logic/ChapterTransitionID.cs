using PVZEngine;

namespace MVZ2Logic
{
    public static class ChapterTransitionID
    {
        public readonly static NamespaceID halloween = Get("halloween");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(Builtin.spaceName, name);
        }
    }
}
