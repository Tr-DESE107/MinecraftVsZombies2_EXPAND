using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Commands
{
    public static class VanillaCommandNames
    {
        public const string help = "help";
        public const string spawn = "spawn";
        public const string kill = "kill";
        public const string blueprint = "blueprint";
    }
    public static class VanillaCommandID
    {
        public static readonly NamespaceID help = Get(VanillaCommandNames.help);
        public static readonly NamespaceID spawn = Get(VanillaCommandNames.spawn);
        public static readonly NamespaceID kill = Get(VanillaCommandNames.kill);
        public static readonly NamespaceID blueprint = Get(VanillaCommandNames.blueprint);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
