﻿using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Shells
{
    public static class VanillaShellNames
    {
        public const string normal = "normal";
        public const string leather = "leather";
        public const string flesh = "flesh";
        public const string bone = "bone";
        public const string stone = "stone";
        public const string grass = "grass";
        public const string metal = "metal";
        public const string wood = "wood";
        public const string nether = "nether";
        public const string diamond = "diamond";
        public const string sand = "sand";
        public const string netherrack = "netherrack";
    }
    public static class VanillaShellID
    {
        public static readonly NamespaceID normal = Get(VanillaShellNames.normal);
        public static readonly NamespaceID leather = Get(VanillaShellNames.leather);
        public static readonly NamespaceID flesh = Get(VanillaShellNames.flesh);
        public static readonly NamespaceID bone = Get(VanillaShellNames.bone);
        public static readonly NamespaceID stone = Get(VanillaShellNames.stone);
        public static readonly NamespaceID grass = Get(VanillaShellNames.grass);
        public static readonly NamespaceID metal = Get(VanillaShellNames.metal);
        public static readonly NamespaceID wood = Get(VanillaShellNames.wood);
        public static readonly NamespaceID nether = Get(VanillaShellNames.nether);
        public static readonly NamespaceID diamond = Get(VanillaShellNames.diamond);
        public static readonly NamespaceID sand = Get(VanillaShellNames.sand);
        public static readonly NamespaceID netherrack = Get(VanillaShellNames.netherrack);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
