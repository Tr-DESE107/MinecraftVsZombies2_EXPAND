using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Artifacts
{
    public static class VanillaArtifactNames
    {
        public const string almanac = "almanac";
        public const string hoe = "hoe";
        public const string dreamKey = "dream_key";
        public const string theCreaturesHeart = "the_creatures_heart";

        public const string ShrunkenHead = "ShrunkenHead";
        public const string FrankensteinBrain = "FrankensteinBrain";

        public const string dreamButterfly = "dream_butterfly";
        public const string sweetSleepPillow = "sweet_sleep_pillow";
        public const string pagodaBranch = "pagoda_branch";
        public const string darkMatter = "dark_matter";
        public const string bottledBlackhole = "bottled_blackhole";

        public const string AngelsFeather = "AngelsFeather";
        public const string NightmareMask = "NightmareMask";

        public const string smartPhone = "smart_phone";
        public const string invertedMirror = "inverted_mirror";
        public const string miracleMalletReplica = "miracle_mallet_replica";
        public const string netherStar = "nether_star";
        public const string witherSkeletonSkull = "wither_skeleton_skull";
        public const string brokenLantern = "broken_lantern";

        public const string WitherHeartShield = "WitherHeartShield";

        public const string manipulativeTalismans = "manipulative_talismans";
        public const string greedyVacuum = "greedy_vacuum";
        public const string lightbomb = "lightbomb";
        public const string eyeOfTheGiant = "eye_of_the_giant";
        public const string ShadowCellCore = "ShadowCellCore";
    }
    public static class VanillaArtifactID
    {
        public static readonly NamespaceID almanac = Get(VanillaArtifactNames.almanac);
        public static readonly NamespaceID dreamKey = Get(VanillaArtifactNames.dreamKey);
        public static readonly NamespaceID theCreaturesHeart = Get(VanillaArtifactNames.theCreaturesHeart);

        public static readonly NamespaceID ShrunkenHead = Get(VanillaArtifactNames.ShrunkenHead);
        public static readonly NamespaceID FrankensteinBrain = Get(VanillaArtifactNames.FrankensteinBrain);

        public static readonly NamespaceID dreamButterfly = Get(VanillaArtifactNames.dreamButterfly);

        public static readonly NamespaceID sweetSleepPillow = Get(VanillaArtifactNames.sweetSleepPillow);
        public static readonly NamespaceID darkMatter = Get(VanillaArtifactNames.darkMatter);
        public static readonly NamespaceID hoe = Get(VanillaArtifactNames.hoe);
        public static readonly NamespaceID pagodaBranch = Get(VanillaArtifactNames.pagodaBranch);
        public static readonly NamespaceID bottledBlackhole = Get(VanillaArtifactNames.bottledBlackhole);

        public static readonly NamespaceID AngelsFeather = Get(VanillaArtifactNames.AngelsFeather);
        public static readonly NamespaceID NightmareMask = Get(VanillaArtifactNames.NightmareMask);

        public static readonly NamespaceID smartPhone = Get(VanillaArtifactNames.smartPhone);
        public static readonly NamespaceID invertedMirror = Get(VanillaArtifactNames.invertedMirror);
        public static readonly NamespaceID miracleMalletReplica = Get(VanillaArtifactNames.miracleMalletReplica);
        public static readonly NamespaceID witherSkeletonSkull = Get(VanillaArtifactNames.witherSkeletonSkull);
        public static readonly NamespaceID netherStar = Get(VanillaArtifactNames.netherStar);
        public static readonly NamespaceID brokenLantern = Get(VanillaArtifactNames.brokenLantern);

        public static readonly NamespaceID WitherHeartShield = Get(VanillaArtifactNames.WitherHeartShield);

        public static readonly NamespaceID manipulativeTalismans = Get(VanillaArtifactNames.manipulativeTalismans);
        public static readonly NamespaceID greedyVacuum = Get(VanillaArtifactNames.greedyVacuum);
        public static readonly NamespaceID lightbomb = Get(VanillaArtifactNames.lightbomb);
        public static readonly NamespaceID eyeOfTheGiant = Get(VanillaArtifactNames.eyeOfTheGiant);
        public static readonly NamespaceID ShadowCellCore = Get(VanillaArtifactNames.ShadowCellCore);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
