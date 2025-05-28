﻿using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2Logic;
using MVZ2Logic.IZombie;
using PVZEngine;
using Tools;

namespace MVZ2.GameContent.IZombie
{
    [IZombieLayoutDefinition(VanillaIZombieLayoutNames.redAlert5)]
    public class RedAlert5 : IZombieLayoutDefinition
    {
        public RedAlert5(string nsp, string name) : base(nsp, name, 5)
        {
            Blueprints = new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.leatherCappedZombie,
                VanillaEnemyID.mesmerizer,
                VanillaEnemyID.berserker,
                VanillaEnemyID.dullahan,
            };
        }
        public override void Fill(IIZombieMap map, RandomGenerator rng)
        {
            for (int i = 0; i < map.Lanes; i++)
            {
                Insert(map, 4, i, VanillaContraptionID.obsidian);
            }
            Insert(map, 3, 0, VanillaContraptionID.glowstone);
            Insert(map, 3, 4, VanillaContraptionID.glowstone);
            Insert(map, 2, 1, VanillaContraptionID.teslaCoil);
            Insert(map, 2, 3, VanillaContraptionID.teslaCoil);

            RandomFill(map, VanillaContraptionID.furnace, 8, rng);
            RandomFill(map, VanillaContraptionID.mineTNT, 4, rng);
            RandomFill(map, VanillaContraptionID.spikeBlock, 4, rng);
        }
    }
}
