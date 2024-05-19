using PVZEngine;

namespace MVZ2.GameContent
{
    public static class StageID
    {
        public static readonly NamespaceID prologue = Get("prologue");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID("mvz2", name);
        }
    }
}
