using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent
{
    public static class ContraptionNames
    {
        public const string dispenser = "dispenser";
        public const string furnace = "furnace";
        public const string obsidian = "obsidian";
        public const string mineTNT = "mine_tnt";

        public const string smallDispenser = "small_dispenser";

        public const string lilyPad = "lily_pad";
    }
    public static class ContraptionID
    {
        public static readonly NamespaceID dispenser = Get(ContraptionNames.dispenser);
        public static readonly NamespaceID furnace = Get(ContraptionNames.furnace);
        public static readonly NamespaceID obsidian = Get(ContraptionNames.obsidian);
        public static readonly NamespaceID mineTNT = Get(ContraptionNames.mineTNT);

        public static readonly NamespaceID smallDispenser = Get(ContraptionNames.smallDispenser);

        public static readonly NamespaceID lilyPad = Get(ContraptionNames.lilyPad);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
