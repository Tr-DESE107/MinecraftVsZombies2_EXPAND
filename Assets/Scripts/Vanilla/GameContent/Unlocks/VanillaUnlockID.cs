using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaUnlockID
    {
        public static readonly NamespaceID almanac = Get("almanac");
        public static readonly NamespaceID store = Get("store");
        public static readonly NamespaceID trigger = Get("trigger");
        public static readonly NamespaceID starshard = Get("starshard");
        public static readonly NamespaceID money = Get("money");
        public static readonly NamespaceID ghostBuster = Get("achievement.ghost_buster");
        public static readonly NamespaceID doubleTrouble = Get("achievement.double_trouble");
        public static readonly NamespaceID rickrollDrown = Get("achievement.rickroll_drown");
        public static readonly NamespaceID returnToSender = Get("achievement.return_to_sender");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
