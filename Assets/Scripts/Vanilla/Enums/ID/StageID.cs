using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class StageNames
    {
        public const string prologue = "prologue";
    }
    public static class StageID
    {
        public static readonly NamespaceID prologue = Get(StageNames.prologue);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
