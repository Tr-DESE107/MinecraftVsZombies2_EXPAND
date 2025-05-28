﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Bosses
{
    public static class VanillaBossNames
    {
        public const string frankenstein = "frankenstein";
        public const string slenderman = "slenderman";
        public const string nightmareaper = "nightmareaper";
        public const string seija = "seija";
        public const string wither = "wither";
        public const string theGiant = "the_giant";
        public const string theGiantSnakeTail = "the_giant_snake_tail";
    }
    public static class VanillaBossID
    {
        public static readonly NamespaceID frankenstein = Get(VanillaBossNames.frankenstein);
        public static readonly NamespaceID slenderman = Get(VanillaBossNames.slenderman);
        public static readonly NamespaceID nightmareaper = Get(VanillaBossNames.nightmareaper);
        public static readonly NamespaceID seija = Get(VanillaBossNames.seija);
        public static readonly NamespaceID wither = Get(VanillaBossNames.wither);
        public static readonly NamespaceID theGiant = Get(VanillaBossNames.theGiant);
        public static readonly NamespaceID theGiantSnakeTail = Get(VanillaBossNames.theGiantSnakeTail);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
