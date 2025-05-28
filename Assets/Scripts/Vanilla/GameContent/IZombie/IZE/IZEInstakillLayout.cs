﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.izeInstakill)]
    public class IZEInstakillLayout : IZELayout
    {
        public IZEInstakillLayout(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void FillEndlessContraptions(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.magichest, 4, rng);
            RandomFill(map, VanillaContraptionID.mineTNT, 6, rng);
            RandomFill(map, VanillaContraptionID.punchton, 2, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 3, rng);
            RandomFill(map, VanillaContraptionID.totenser, 2, rng);
        }
    }
}
