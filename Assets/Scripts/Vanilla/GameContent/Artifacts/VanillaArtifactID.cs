using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Artifacts
{
    public static class VanillaArtifactNames
    {
        public const string dreamKey = "dream_key";
    }
    public static class VanillaArtifactID
    {
        public static readonly NamespaceID dreamKey = Get(VanillaArtifactNames.dreamKey);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
