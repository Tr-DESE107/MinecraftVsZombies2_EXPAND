﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.HeldItems
{
    public static class VanillaHeldItemNames
    {
        public const string pickaxe = "pickaxe";
        public const string starshard = "starshard";
        public const string trigger = "trigger";
        public const string sword = "sword";
        public const string forcePad = "force_pad";
        public const string breakoutBoard = "breakout_board";
        public const string blueprintPickup = "blueprint_pickup";
    }
    public static class VanillaHeldTypes
    {
        public static readonly NamespaceID pickaxe = Get(VanillaHeldItemNames.pickaxe);
        public static readonly NamespaceID starshard = Get(VanillaHeldItemNames.starshard);
        public static readonly NamespaceID trigger = Get(VanillaHeldItemNames.trigger);
        public static readonly NamespaceID sword = Get(VanillaHeldItemNames.sword);
        public static readonly NamespaceID forcePad = Get(VanillaHeldItemNames.forcePad);
        public static readonly NamespaceID breakoutBoard = Get(VanillaHeldItemNames.breakoutBoard);
        public static readonly NamespaceID blueprintPickup = Get(VanillaHeldItemNames.blueprintPickup);
        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
