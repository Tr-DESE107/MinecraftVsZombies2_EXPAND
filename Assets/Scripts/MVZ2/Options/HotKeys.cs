﻿using MVZ2.Managers;
using PVZEngine;

namespace MVZ2.Options
{
    public static class HotKeys
    {
        public static readonly NamespaceID pickaxe = Get("pickaxe");
        public static readonly NamespaceID starshard = Get("starshard");
        public static readonly NamespaceID trigger = Get("trigger");
        public static readonly NamespaceID fastForward = Get("fast_forward");
        public static readonly NamespaceID blueprint1 = Get("blueprint_1");
        public static readonly NamespaceID blueprint2 = Get("blueprint_2");
        public static readonly NamespaceID blueprint3 = Get("blueprint_3");
        public static readonly NamespaceID blueprint4 = Get("blueprint_4");
        public static readonly NamespaceID blueprint5 = Get("blueprint_5");
        public static readonly NamespaceID blueprint6 = Get("blueprint_6");
        public static readonly NamespaceID blueprint7 = Get("blueprint_7");
        public static readonly NamespaceID blueprint8 = Get("blueprint_8");
        public static readonly NamespaceID blueprint9 = Get("blueprint_9");
        public static readonly NamespaceID blueprint10 = Get("blueprint_10");
        private static readonly NamespaceID[] blueprintList = new NamespaceID[]
        {
            blueprint1,
            blueprint2,
            blueprint3,
            blueprint4,
            blueprint5,
            blueprint6,
            blueprint7,
            blueprint8,
            blueprint9,
            blueprint10,
        };
        public static NamespaceID GetBlueprintHotKey(int index)
        {
            return blueprintList[index];
        }
        public static NamespaceID Get(string path) => new NamespaceID(MainManager.Instance.BuiltinNamespace, path);
    }
}
