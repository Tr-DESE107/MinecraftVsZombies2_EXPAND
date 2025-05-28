﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.izeExplosives)]
    public class IZEExplosivesLayout : IZELayout
    {
        public IZEExplosivesLayout(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void FillEndlessContraptions(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.magichest, 4, rng);
            RandomFill(map, VanillaContraptionID.mineTNT, 13, rng);
        }
    }
}
