using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Commands
{
    public static class VanillaCommandNames
    {
        public const string help = "help";
        public const string spawn = "spawn";
    }
    public static class VanillaCommandID
    {
        public static readonly NamespaceID help = Get(VanillaCommandNames.help);
        public static readonly NamespaceID spawn = Get(VanillaCommandNames.spawn);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
