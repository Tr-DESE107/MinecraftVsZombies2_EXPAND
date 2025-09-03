using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Armors
{
    public static class VanillaArmorNames
    {
        public const string leatherCap = "leather_cap";
        public const string ironHelmet = "iron_helmet";

        public const string SandstoneHelmet = "SandstoneHelmet";
        public const string bone_helmet = "bone_helmet";
        public const string necromancer_helmet = "necromancer_helmet";

        public const string mesmerizerCrown = "mesmerizer_crown";
        public const string berserkerHelmet = "berserker_helmet";
        public const string bedserkerHelmet = "bedserker_helmet";

        public const string nether_helmet = "nether_helmet";
        public const string netherite_ingot_helmet = "netherite_ingot_helmet";
        public const string AngryReverserCrown = "AngryReverserCrown";
        public const string KingofReverserCrown = "KingofReverserCrown";

        public const string reflectiveBarrier = "reflective_barrier";
        public const string wickedHermitHat = "wicked_hermit_hat";
        public const string skeletonWarriorHelmet = "skeleton_warrior_helmet";
        public const string skeletonWarriorShield = "skeleton_warrior_shield";
        public const string emperorCrown = "emperor_crown";

        public const string umbrellaShield = "umbrella_shield";
    }
    public static class VanillaArmorID
    {
        public static readonly NamespaceID leatherCap = Get(VanillaArmorNames.leatherCap);
        public static readonly NamespaceID ironHelmet = Get(VanillaArmorNames.ironHelmet);

        public static readonly NamespaceID SandstoneHelmet = Get(VanillaArmorNames.SandstoneHelmet);
        public static readonly NamespaceID necromancer_helmet = Get(VanillaArmorNames.necromancer_helmet);
        public static readonly NamespaceID bone_helmet = Get(VanillaArmorNames.bone_helmet);

        public static readonly NamespaceID mesmerizerCrown = Get(VanillaArmorNames.mesmerizerCrown);
        public static readonly NamespaceID bersekerHelmet = Get(VanillaArmorNames.berserkerHelmet);
        public static readonly NamespaceID bedserkerHelmet = Get(VanillaArmorNames.bedserkerHelmet);

        public static readonly NamespaceID nether_helmet = Get(VanillaArmorNames.nether_helmet);
        public static readonly NamespaceID netherite_ingot_helmet = Get(VanillaArmorNames.netherite_ingot_helmet);
        public static readonly NamespaceID AngryReverserCrown = Get(VanillaArmorNames.AngryReverserCrown);
        public static readonly NamespaceID KingofReverserCrown = Get(VanillaArmorNames.KingofReverserCrown);

        public static readonly NamespaceID reflectiveBarrier = Get(VanillaArmorNames.reflectiveBarrier);
        public static readonly NamespaceID wickedHermitHat = Get(VanillaArmorNames.wickedHermitHat);
        public static readonly NamespaceID skeletonWarriorHelmet = Get(VanillaArmorNames.skeletonWarriorHelmet);
        public static readonly NamespaceID skeletonWarriorShield = Get(VanillaArmorNames.skeletonWarriorShield);
        public static readonly NamespaceID emperorCrown = Get(VanillaArmorNames.emperorCrown);

        public static readonly NamespaceID umbrellaShield = Get(VanillaArmorNames.umbrellaShield);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
