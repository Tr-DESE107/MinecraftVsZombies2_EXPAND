﻿using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.puzzleUnbreakable)]
    public class PuzzleUnbreakableLayout : IZombieLayoutDefinition
    {
        public PuzzleUnbreakableLayout(string nsp, string name) : base(nsp, name, 5)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.gargoyle,
                VanillaEnemyID.dullahan,
                VanillaEnemyID.hellChariot
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            int[] oddLanes = new int[3];
            for (int i = 0; i < oddLanes.Length; i++)
            {
                oddLanes[i] = i;
            }
            var shuffledOddLanes = oddLanes.Shuffle(rng);
            for (int lane = 0; lane < map.Lanes; lane++)
            {
                bool odd = lane % 2 == 0;
                for (int column = 0; column < Columns; column++)
                {
                    NamespaceID id;
                    if (odd)
                    {
                        id = column <= 1 ? VanillaContraptionID.furnace : VanillaContraptionID.stoneShield;
                    }
                    else
                    {
                        id = column >= 3 ? VanillaContraptionID.furnace : VanillaContraptionID.stoneShield;
                    }
                    Insert(map, column, lane, id);
                }
                if (odd)
                {
                    var contraptionLane = shuffledOddLanes.ElementAtOrDefault(lane / 2);
                    switch (contraptionLane)
                    {
                        case LANE_TOTENSERS:
                            RandomFillAtLane(map, lane, VanillaContraptionID.totenser, 3, rng);
                            break;
                        case LANE_SILVENSERS:
                            RandomFillAtLane(map, lane, VanillaContraptionID.silvenser, 3, rng);
                            break;
                        case LANE_DRIVENSERS:
                            RandomFillAtLane(map, lane, VanillaContraptionID.drivenser, 3, rng);
                            break;
                    }
                }
                else
                {
                    RandomFillAtLane(map, lane, VanillaContraptionID.punchton, 1, rng);
                    RandomFillAtLane(map, lane, VanillaContraptionID.spikeBlock, 1, rng);
                    RandomFillAtLane(map, lane, VanillaContraptionID.mineTNT, 1, rng);
                }
            }
        }
        public const int LANE_TOTENSERS = 0;
        public const int LANE_SILVENSERS = 1;
        public const int LANE_DRIVENSERS = 2;
    }
}
