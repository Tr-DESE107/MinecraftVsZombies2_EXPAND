using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.GameContent
{
    public static class VanillaNoteNames
    {
        public const string prologue = "prologue";
        public const string halloween = "halloween";
    }
    public static class VanillaNoteID
    {
        public static readonly NamespaceID prologue = Get(VanillaNoteNames.prologue);
        public static readonly NamespaceID halloween = Get(VanillaNoteNames.halloween);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
