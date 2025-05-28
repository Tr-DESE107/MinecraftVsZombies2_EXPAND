﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleISkeleton)]
    public class PuzzleISkeletonLayout : IZombieLayoutDefinition
    {
        public PuzzleISkeletonLayout(string nsp, string name) : base(nsp, name, 4)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.skeleton,
                VanillaEnemyID.ghost,
                VanillaEnemyID.necromancer
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            Insert(map, 2, 2, VanillaContraptionID.glowstone);
            RandomFill(map, VanillaContraptionID.punchton, 5, rng);
            RandomFill(map, VanillaContraptionID.silvenser, 3, rng);
            RandomFill(map, VanillaContraptionID.dispenser, 3, rng);
            RandomFill(map, VanillaContraptionID.furnace, 8, rng);
        }
    }
}
