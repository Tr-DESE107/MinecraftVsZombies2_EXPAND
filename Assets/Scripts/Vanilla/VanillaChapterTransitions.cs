using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaChapterTransitions
    {
        public readonly static NamespaceID halloween = Get("halloween");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
