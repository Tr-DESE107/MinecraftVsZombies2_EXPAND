using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Stages
{
    public static class VanillaStageNames
    {
        public const string debug = "debug";
        public const string tutorial = "tutorial";
        public const string starshardTutorial = "starshard_tutorial";
        public const string triggerTutorial = "trigger_tutorial";

        public const string prologue = "prologue";

        public const string halloween1 = "halloween_1";
        public const string halloween2 = "halloween_2";
        public const string halloween3 = "halloween_3";
        public const string halloween4 = "halloween_4";
        public const string halloween5 = "halloween_5";
        public const string halloween6 = "halloween_6";
        public const string halloween7 = "halloween_7";
        public const string halloween8 = "halloween_8";
        public const string halloween9 = "halloween_9";
        public const string halloween10 = "halloween_10";
        public const string halloween11 = "halloween_11";
        public const string halloweenEndless = "halloween_endless";

        public const string dream1 = "dream_1";
        public const string dream2 = "dream_2";
        public const string dream3 = "dream_3";
        public const string dream4 = "dream_4";
        public const string dream5 = "dream_5";
        public const string dream6 = "dream_6";
        public const string dream7 = "dream_7";
        public const string dream8 = "dream_8";
        public const string dream9 = "dream_9";
        public const string dream10 = "dream_10";
        public const string dream11 = "dream_11";
        public const string dreamEndless = "dream_endless";

        public const string teruharijou1 = "teruharijou_1";
        public const string teruharijou2 = "teruharijou_2";
        public const string teruharijou3 = "teruharijou_3";
        public const string teruharijou4 = "teruharijou_4";
        public const string teruharijou5 = "teruharijou_5";
        public const string teruharijou6 = "teruharijou_6";
        public const string teruharijou7 = "teruharijou_7";
        public const string teruharijou8 = "teruharijou_8";
        public const string teruharijou9 = "teruharijou_9";
        public const string teruharijou10 = "teruharijou_10";
        public const string teruharijou11 = "teruharijou_11";
        public const string teruharijouEndless = "teruharijou_endless";
    }
    public static class VanillaStageID
    {
        public static readonly NamespaceID debug = Get(VanillaStageNames.debug);
        public static readonly NamespaceID prologue = Get(VanillaStageNames.prologue);
        public static readonly NamespaceID tutorial = Get(VanillaStageNames.tutorial);
        public static readonly NamespaceID starshardTutorial = Get(VanillaStageNames.starshardTutorial);
        public static readonly NamespaceID triggerTutorial = Get(VanillaStageNames.triggerTutorial);
        public static readonly NamespaceID halloween1 = Get(VanillaStageNames.halloween1);
        public static readonly NamespaceID halloween2 = Get(VanillaStageNames.halloween2);
        public static readonly NamespaceID halloween7 = Get(VanillaStageNames.halloween7);
        public static readonly NamespaceID halloween10 = Get(VanillaStageNames.halloween10);
        public static readonly NamespaceID halloween11 = Get(VanillaStageNames.halloween11);
        public static readonly NamespaceID dream1 = Get(VanillaStageNames.dream1);
        public static readonly NamespaceID dream11 = Get(VanillaStageNames.dream11);
        public static readonly NamespaceID teruharijou1 = Get(VanillaStageNames.teruharijou1);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
