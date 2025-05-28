﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Areas
{
    public static class VanillaAreaNames
    {
        public const string day = "day";
        public const string halloween = "halloween";
        public const string dream = "dream";
        public const string castle = "castle";
        public const string mausoleum = "mausoleum";
        public const string mausoleumMinigame = "mausoleum_minigame";
    }
    public static class VanillaAreaID
    {
        public static readonly NamespaceID day = Get(VanillaAreaNames.day);
        public static readonly NamespaceID halloween = Get(VanillaAreaNames.halloween);
        public static readonly NamespaceID dream = Get(VanillaAreaNames.dream);
        public static readonly NamespaceID castle = Get(VanillaAreaNames.castle);
        public static readonly NamespaceID mausoleum = Get(VanillaAreaNames.mausoleum);
        public static readonly NamespaceID mausoleumMinigame = Get(VanillaAreaNames.mausoleumMinigame);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
