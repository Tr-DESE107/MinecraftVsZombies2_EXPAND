using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Contraptions
{
    public static class VanillaContraptionNames
    {
        public const string dispenser = "dispenser";
        public const string furnace = "furnace";
        public const string obsidian = "obsidian";
        public const string mineTNT = "mine_tnt";

        public const string smallDispenser = "small_dispenser";
        public const string moonlightSensor = "moonlight_sensor";
        public const string glowstone = "glowstone";
        public const string punchton = "punchton";
        public const string tnt = "tnt";

        public const string lilyPad = "lily_pad";
    }
    public static class VanillaContraptionID
    {
        public static readonly NamespaceID dispenser = Get(VanillaContraptionNames.dispenser);
        public static readonly NamespaceID furnace = Get(VanillaContraptionNames.furnace);
        public static readonly NamespaceID obsidian = Get(VanillaContraptionNames.obsidian);
        public static readonly NamespaceID mineTNT = Get(VanillaContraptionNames.mineTNT);

        public static readonly NamespaceID smallDispenser = Get(VanillaContraptionNames.smallDispenser);
        public static readonly NamespaceID moonlightSensor = Get(VanillaContraptionNames.moonlightSensor);
        public static readonly NamespaceID glowstone = Get(VanillaContraptionNames.glowstone);
        public static readonly NamespaceID punchton = Get(VanillaContraptionNames.punchton);
        public static readonly NamespaceID tnt = Get(VanillaContraptionNames.tnt);

        public static readonly NamespaceID lilyPad = Get(VanillaContraptionNames.lilyPad);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
