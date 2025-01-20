using MVZ2.GameContent.Stages;
using MVZ2.Vanilla.Saves;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaUnlockNames
    {
        public const string blueprintSlot1 = "blueprint_slot.1";
        public const string blueprintSlot2 = "blueprint_slot.2";
        public const string blueprintSlot3 = "blueprint_slot.3";
        public const string blueprintSlot4 = "blueprint_slot.4";

        public const string starshardSlot1 = "starshard_slot.1";
        public const string starshardSlot2 = "starshard_slot.2";

        public const string artifactSlot1 = "artifact_slot.1";
        public const string artifactSlot2 = "artifact_slot.2";

        public const string infectenser = "contraption.infectenser";
        public const string forcePad = "contraption.force_pad";

        public const string money = "money";
        public const string starshard = "starshard";
        public const string trigger = "trigger";

        public const string enteredDream = "entered_dream";
        public const string mapStoreArrow = "map_store_arrow";

        public const string dreamIsNightmare = "dream_is_nightmare";

        public const string ghostBuster = "achievement.ghost_buster";
        public const string doubleTrouble = "achievement.double_trouble";
        public const string rickrollDrown = "achievement.rickroll_drown";
        public const string returnToSender = "achievement.return_to_sender";
    }
    public static class VanillaUnlockID
    {
        public static readonly NamespaceID halloween5 = GetStage(VanillaStageNames.halloween5);
        public static readonly NamespaceID halloween11 = GetStage(VanillaStageNames.halloween11);
        public static readonly NamespaceID dream5 = GetStage(VanillaStageNames.dream5);
        public static readonly NamespaceID dream11 = GetStage(VanillaStageNames.dream11);
        public static readonly NamespaceID almanac = halloween5;
        public static readonly NamespaceID store = dream5;
        public static readonly NamespaceID gensokyo = dream11;
        public static readonly NamespaceID trigger = Get(VanillaUnlockNames.trigger);
        public static readonly NamespaceID starshard = Get(VanillaUnlockNames.starshard);
        public static readonly NamespaceID money = Get(VanillaUnlockNames.money);
        public static readonly NamespaceID ghostBuster = Get(VanillaUnlockNames.ghostBuster);
        public static readonly NamespaceID doubleTrouble = Get(VanillaUnlockNames.doubleTrouble);
        public static readonly NamespaceID rickrollDrown = Get(VanillaUnlockNames.rickrollDrown);
        public static readonly NamespaceID returnToSender = Get(VanillaUnlockNames.returnToSender);
        public static readonly NamespaceID enteredDream = Get(VanillaUnlockNames.enteredDream);
        public static readonly NamespaceID dreamIsNightmare = Get(VanillaUnlockNames.dreamIsNightmare);
        public static readonly NamespaceID blueprintSlot1 = Get(VanillaUnlockNames.blueprintSlot1);
        public static readonly NamespaceID mapStoreArrow = Get(VanillaUnlockNames.mapStoreArrow);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
        private static NamespaceID GetStage(string name)
        {
            return Get(VanillaSaveExt.GetLevelClearUnlockID(name));
        }
    }
}
