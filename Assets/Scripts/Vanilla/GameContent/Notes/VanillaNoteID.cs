﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Notes
{
    public static class VanillaNoteNames
    {
        public const string help = "help";
        public const string prologue = "prologue";
        public const string halloween = "halloween";
        public const string dream = "dream";
        public const string castle = "castle";
        public const string mausoleum = "mausoleum";
    }
    public static class VanillaNoteID
    {
        public static readonly NamespaceID help = Get(VanillaNoteNames.help);
        public static readonly NamespaceID prologue = Get(VanillaNoteNames.prologue);
        public static readonly NamespaceID halloween = Get(VanillaNoteNames.halloween);
        public static readonly NamespaceID dream = Get(VanillaNoteNames.dream);
        public static readonly NamespaceID castle = Get(VanillaNoteNames.castle);
        public static readonly NamespaceID mausoleum = Get(VanillaNoteNames.mausoleum);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
