﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Armors
{
    public static class VanillaArmorSlots
    {
        public static readonly NamespaceID main = Get("main");
        public static readonly NamespaceID shield = Get("shield");
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
