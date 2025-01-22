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
        public const string soulFurnace = "soul_furnace";
        public const string silvenser = "silvenser";
        public const string magichest = "magichest";

        public const string lilyPad = "lily_pad";
        public const string drivenser = "drivenser";
        public const string gravityPad = "gravity_pad";
        public const string vortexHopper = "vortex_hopper";
        public const string pistenser = "pistenser";
        public const string totenser = "totenser";
        public const string dreamCrystal = "dream_crystal";
        public const string dreamSilk = "dream_silk";

        public const string woodenDropper = "wooden_dropper";
        public const string spikeBlock = "spike_block";
        public const string stoneDropper = "stone_dropper";
        public const string stoneShield = "stone_shield";

        public const string infectenser = "infectenser";
        public const string forcePad = "force_pad";
        public const string goldenDropper = "golden_dropper";
        public const string diamondSpikes = "diamond_spikes";

        public const string anvil = "anvil";
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
        public static readonly NamespaceID soulFurnace = Get(VanillaContraptionNames.soulFurnace);
        public static readonly NamespaceID silvenser = Get(VanillaContraptionNames.silvenser);
        public static readonly NamespaceID magichest = Get(VanillaContraptionNames.magichest);

        public static readonly NamespaceID lilyPad = Get(VanillaContraptionNames.lilyPad);
        public static readonly NamespaceID drivenser = Get(VanillaContraptionNames.drivenser);
        public static readonly NamespaceID gravityPad = Get(VanillaContraptionNames.gravityPad);
        public static readonly NamespaceID vortexHopper = Get(VanillaContraptionNames.vortexHopper);
        public static readonly NamespaceID pistenser = Get(VanillaContraptionNames.pistenser);
        public static readonly NamespaceID totenser = Get(VanillaContraptionNames.totenser);
        public static readonly NamespaceID dreamCrystal = Get(VanillaContraptionNames.dreamCrystal);
        public static readonly NamespaceID dreamSilk = Get(VanillaContraptionNames.dreamSilk);

        public static readonly NamespaceID woodenDropper = Get(VanillaContraptionNames.woodenDropper);
        public static readonly NamespaceID spikeBlock = Get(VanillaContraptionNames.spikeBlock);
        public static readonly NamespaceID stoneDropper = Get(VanillaContraptionNames.stoneDropper);
        public static readonly NamespaceID stoneShield = Get(VanillaContraptionNames.stoneShield);

        public static readonly NamespaceID infectenser = Get(VanillaContraptionNames.infectenser);
        public static readonly NamespaceID forcePad = Get(VanillaContraptionNames.forcePad);
        public static readonly NamespaceID goldenDropper = Get(VanillaContraptionNames.goldenDropper);
        public static readonly NamespaceID diamondSpikes = Get(VanillaContraptionNames.diamondSpikes);

        public static readonly NamespaceID anvil = Get(VanillaContraptionNames.anvil);
        private static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
