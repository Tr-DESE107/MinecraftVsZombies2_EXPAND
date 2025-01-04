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
    }
    public static class VanillaUnlockID
    {
        public static readonly NamespaceID halloween5 = Get("level.halloween_5");
        public static readonly NamespaceID halloween11 = Get("level.halloween_11");
        public static readonly NamespaceID dream5 = Get("level.dream_5");
        public static readonly NamespaceID almanac = halloween5;
        public static readonly NamespaceID store = dream5;
        public static readonly NamespaceID trigger = Get("trigger");
        public static readonly NamespaceID starshard = Get("starshard");
        public static readonly NamespaceID money = Get("money");
        public static readonly NamespaceID ghostBuster = Get("achievement.ghost_buster");
        public static readonly NamespaceID doubleTrouble = Get("achievement.double_trouble");
        public static readonly NamespaceID rickrollDrown = Get("achievement.rickroll_drown");
        public static readonly NamespaceID returnToSender = Get("achievement.return_to_sender");
        public static readonly NamespaceID enteredDream = Get("entered_dream");
        public static readonly NamespaceID dreamIsNightmare = Get("dream_is_nightmare");
        public static readonly NamespaceID blueprintSlot1 = Get(VanillaUnlockNames.blueprintSlot1);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
