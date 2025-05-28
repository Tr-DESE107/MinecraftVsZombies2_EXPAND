﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Talk
{
    public static class VanillaTalkNames
    {
        public const string tutorial = "tutorial";
        public const string starshardTutorial = "starshard_tutorial";
        public const string triggerTutorial = "trigger_tutorial";
        public const string halloween7 = "halloween_7";
        public const string halloweenFinal = "halloween_final";
        public const string castle7Boss = "castle_7_boss";
    }
    public static class VanillaTalkID
    {
        public static readonly NamespaceID tutorial = Get(VanillaTalkNames.tutorial);
        public static readonly NamespaceID starshardTutorial = Get(VanillaTalkNames.starshardTutorial);
        public static readonly NamespaceID triggerTutorial = Get(VanillaTalkNames.triggerTutorial);
        public static readonly NamespaceID halloween7 = Get(VanillaTalkNames.halloween7);
        public static readonly NamespaceID halloweenFinal = Get(VanillaTalkNames.halloweenFinal);
        public static readonly NamespaceID castle7Boss = Get(VanillaTalkNames.castle7Boss);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
