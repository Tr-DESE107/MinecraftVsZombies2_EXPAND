using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class FragmentNames
    {
        public const string obsidianArmor = "obsidian_armor";
    }
    public static class FragmentID
    {
        public static readonly NamespaceID obsidianArmor = Get(FragmentNames.obsidianArmor);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
