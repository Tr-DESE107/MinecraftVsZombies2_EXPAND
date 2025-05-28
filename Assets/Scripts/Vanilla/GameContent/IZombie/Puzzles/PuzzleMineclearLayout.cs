﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleMineclear)]
    public class PuzzleMineclearLayout : IZombieLayoutDefinition
    {
        public PuzzleMineclearLayout(string nsp, string name) : base(nsp, name, 5)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.imp,
                VanillaEnemyID.necromancer,
                VanillaEnemyID.emperorZombie
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            RandomFill(map, VanillaContraptionID.mineTNT, 9, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 3, rng);
            RandomFill(map, VanillaContraptionID.punchton, 5, rng);
            RandomFill(map, VanillaContraptionID.furnace, 8, rng);
        }
    }
}
