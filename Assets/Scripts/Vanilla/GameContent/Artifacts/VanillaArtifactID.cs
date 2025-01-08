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

        public const string dreamButterfly = "dream_butterfly";
        public const string sweetSleepPillow = "sweet_sleep_pillow";
        public const string pagodaBranch = "pagoda_branch";
        public const string darkMatter = "dark_matter";
    }
    public static class VanillaArtifactID
    {
        public static readonly NamespaceID almanac = Get(VanillaArtifactNames.almanac);
        public static readonly NamespaceID dreamKey = Get(VanillaArtifactNames.dreamKey);
        public static readonly NamespaceID theCreaturesHeart = Get(VanillaArtifactNames.theCreaturesHeart);
        public static readonly NamespaceID dreamButterfly = Get(VanillaArtifactNames.dreamButterfly);

        public static readonly NamespaceID sweetSleepPillow = Get(VanillaArtifactNames.sweetSleepPillow);
        public static readonly NamespaceID darkMatter = Get(VanillaArtifactNames.darkMatter);
        public static readonly NamespaceID hoe = Get(VanillaArtifactNames.hoe);
        public static readonly NamespaceID pagodaBranch = Get(VanillaArtifactNames.pagodaBranch);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
