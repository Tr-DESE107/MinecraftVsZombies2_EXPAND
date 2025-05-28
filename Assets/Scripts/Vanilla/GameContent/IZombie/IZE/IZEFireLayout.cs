﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.izeFire)]
    public class IZEFireLayout : IZELayout
    {
        public IZEFireLayout(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void FillEndlessContraptions(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.smallDispenser, 3, rng);
            RandomFill(map, VanillaContraptionID.soulFurnace, 5, rng);
            RandomFill(map, VanillaContraptionID.dispenser, 2, rng);
            RandomFill(map, VanillaContraptionID.splitenser, 2, rng);
            RandomFill(map, VanillaContraptionID.totenser, 2, rng);
            RandomFill(map, VanillaContraptionID.hellfire, 3, rng);
        }
    }
}
