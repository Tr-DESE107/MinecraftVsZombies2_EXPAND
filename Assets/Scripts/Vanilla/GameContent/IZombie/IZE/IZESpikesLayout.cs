﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.izeSpikes)]
    public class IZESpikesLayout : IZELayout
    {
        public IZESpikesLayout(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void FillEndlessContraptions(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.woodenDropper, 8, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 9, rng);
        }
    }
}
