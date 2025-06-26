﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.iZombieDebug)]
    public class IZombieDebugLayout : IZombieLayoutDefinition
    {
        public IZombieDebugLayout(string nsp, string name) : base(nsp, name, 4)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.imp,
                VanillaContraptionID.spikeBlock,
                VanillaContraptionID.smallDispenser
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            for (int lane = 0; lane < map.Lanes; lane++)
            {
                Insert(map, 2, lane, VanillaContraptionID.spikeBlock);
                Insert(map, 3, lane, VanillaContraptionID.smallDispenser);
            }
        }
    }
}
