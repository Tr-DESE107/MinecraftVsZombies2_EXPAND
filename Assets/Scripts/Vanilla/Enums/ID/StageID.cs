using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class StageNames
    {
        public const string tutorial = "tutorial";
        public const string starshard_tutorial = "starshard_tutorial";
        public const string trigger_tutorial = "trigger_tutorial";
        public const string prologue = "prologue";
        public const string halloween7 = "halloween7";
    }
    public static class StageID
    {
        public static readonly NamespaceID prologue = Get(StageNames.prologue);
        public static readonly NamespaceID tutorial = Get(StageNames.tutorial);
        public static readonly NamespaceID starshard_tutorial = Get(StageNames.starshard_tutorial);
        public static readonly NamespaceID trigger_tutorial = Get(StageNames.trigger_tutorial);
        public static readonly NamespaceID halloween7 = Get(StageNames.halloween7);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
