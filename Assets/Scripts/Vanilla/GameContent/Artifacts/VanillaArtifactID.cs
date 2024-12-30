using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Artifacts
{
    public static class VanillaArtifactNames
    {
        public const string dreamKey = "dream_key";
        public const string theCreaturesHeart = "the_creatures_heart";
    }
    public static class VanillaArtifactID
    {
        public static readonly NamespaceID dreamKey = Get(VanillaArtifactNames.dreamKey);
        public static readonly NamespaceID theCreaturesHeart = Get(VanillaArtifactNames.theCreaturesHeart);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
