using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Stages
{
    public static class VanillaStageNames
    {
        public const string debug = "debug";
        public const string tutorial = "tutorial";
        public const string starshardTutorial = "starshard_tutorial";
        public const string trigger_tutorial = "trigger_tutorial";
        public const string prologue = "prologue";
        public const string halloween1 = "halloween_1";
        public const string halloween2 = "halloween_2";
        public const string halloween7 = "halloween_7";
    }
    public static class VanillaStageID
    {
        public static readonly NamespaceID debug = Get(VanillaStageNames.debug);
        public static readonly NamespaceID prologue = Get(VanillaStageNames.prologue);
        public static readonly NamespaceID tutorial = Get(VanillaStageNames.tutorial);
        public static readonly NamespaceID starshard_tutorial = Get(VanillaStageNames.starshardTutorial);
        public static readonly NamespaceID trigger_tutorial = Get(VanillaStageNames.trigger_tutorial);
        public static readonly NamespaceID halloween1 = Get(VanillaStageNames.halloween1);
        public static readonly NamespaceID halloween2 = Get(VanillaStageNames.halloween2);
        public static readonly NamespaceID halloween7 = Get(VanillaStageNames.halloween7);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
