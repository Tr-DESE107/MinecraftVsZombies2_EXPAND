﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Commands
{
    public static class VanillaCommandNames
    {
        public const string help = "help";
        public const string spawn = "spawn";
        public const string spawnBlueprint = "spawnblueprint";
        public const string kill = "kill";
        public const string blueprint = "blueprint";
        public const string energy = "energy";
        public const string starshard = "starshard";
        public const string recharge = "recharge";
        public const string cheat = "cheat";
        public const string repeat = "repeat";
        public const string clear = "clear";
        public const string clearLevel = "clearlevel";
        public const string save = "save";
        public const string load = "load";
        public const string artifact = "artifact";
    }
    public static class VanillaCommandID
    {
        public static readonly NamespaceID help = Get(VanillaCommandNames.help);
        public static readonly NamespaceID spawn = Get(VanillaCommandNames.spawn);
        public static readonly NamespaceID spawnBlueprint = Get(VanillaCommandNames.spawnBlueprint);
        public static readonly NamespaceID kill = Get(VanillaCommandNames.kill);
        public static readonly NamespaceID blueprint = Get(VanillaCommandNames.blueprint);
        public static readonly NamespaceID energy = Get(VanillaCommandNames.energy);
        public static readonly NamespaceID starshard = Get(VanillaCommandNames.starshard);
        public static readonly NamespaceID recharge = Get(VanillaCommandNames.recharge);
        public static readonly NamespaceID cheat = Get(VanillaCommandNames.cheat);
        public static readonly NamespaceID repeat = Get(VanillaCommandNames.repeat);
        public static readonly NamespaceID clear = Get(VanillaCommandNames.clear);
        public static readonly NamespaceID clearLevel = Get(VanillaCommandNames.clearLevel);
        public static readonly NamespaceID save = Get(VanillaCommandNames.save);
        public static readonly NamespaceID load = Get(VanillaCommandNames.load);
        public static readonly NamespaceID artifact = Get(VanillaCommandNames.artifact);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
