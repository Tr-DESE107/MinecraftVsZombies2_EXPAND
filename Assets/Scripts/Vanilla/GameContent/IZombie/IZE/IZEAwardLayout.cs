﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.izeAwards)]
    public class IZEAwardLayout : IZELayout
    {
        public IZEAwardLayout(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void FillEndlessContraptions(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.mineTNT, 4, rng);
            RandomFill(map, VanillaContraptionID.smallDispenser, 4, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 4, rng);
            RandomFill(map, VanillaContraptionID.furnace, 5, rng);
        }
    }
}
