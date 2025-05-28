﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.izeComposite)]
    public class IZECompositeLayout : IZELayout
    {
        public IZECompositeLayout(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void FillEndlessContraptions(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.dispenser, 1, rng);
            RandomFill(map, VanillaContraptionID.obsidian, 1, rng);
            RandomFill(map, VanillaContraptionID.mineTNT, 1, rng);
            RandomFill(map, VanillaContraptionID.glowstone, 1, rng);
            RandomFill(map, VanillaContraptionID.punchton, 1, rng);
            RandomFill(map, VanillaContraptionID.soulFurnace, 1, rng);
            RandomFill(map, VanillaContraptionID.silvenser, 1, rng);
            RandomFill(map, VanillaContraptionID.magichest, 1, rng);
            RandomFill(map, VanillaContraptionID.drivenser, 1, rng);
            RandomFill(map, VanillaContraptionID.gravityPad, 1, rng); // Does not count
            RandomFill(map, VanillaContraptionID.totenser, 1, rng);
            RandomFill(map, VanillaContraptionID.dreamCrystal, 1, rng);
            RandomFill(map, VanillaContraptionID.woodenDropper, 1, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 1, rng);
            RandomFill(map, VanillaContraptionID.stoneDropper, 1, rng);
            RandomFill(map, VanillaContraptionID.splitenser, 2, rng);
            RandomFill(map, VanillaContraptionID.hellfire, 1, rng);
        }
    }
}
