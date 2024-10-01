using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class VanillaFragmentNames
    {
        public const string obsidianArmor = "obsidian_armor";
    }
    public static class VanillaFragmentID
    {
        public static readonly NamespaceID obsidianArmor = Get(VanillaFragmentNames.obsidianArmor);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
