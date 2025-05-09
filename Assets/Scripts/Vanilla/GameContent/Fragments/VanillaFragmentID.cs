using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Fragments
{
    public static class VanillaFragmentNames
    {
        public const string obsidianArmor = "obsidian_armor";
        public const string reflectiveBarrier = "reflective_barrier";
        public const string hellfireCursed = "hellfire_cursed";
    }
    public static class VanillaFragmentID
    {
        public static readonly NamespaceID obsidianArmor = Get(VanillaFragmentNames.obsidianArmor);
        public static readonly NamespaceID reflectiveBarrier = Get(VanillaFragmentNames.reflectiveBarrier);
        public static readonly NamespaceID hellfireCursed = Get(VanillaFragmentNames.hellfireCursed);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
