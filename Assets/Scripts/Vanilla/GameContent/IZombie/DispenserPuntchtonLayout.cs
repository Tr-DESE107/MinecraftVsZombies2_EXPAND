﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.dispenserPunchton4)]
    public class DispenserPuntchtonLayout : IZombieLayoutDefinition
    {
        public DispenserPuntchtonLayout(string nsp, string name) : base(nsp, name, 4)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.gargoyle,
                VanillaEnemyID.ironHelmettedZombie
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.dispenser, 5, rng);
            RandomFill(map, VanillaContraptionID.furnace, 8, rng);
            RandomFill(map, VanillaContraptionID.silvenser, 4, rng);
            RandomFill(map, VanillaContraptionID.punchton, 3, rng);
        }
    }
}
