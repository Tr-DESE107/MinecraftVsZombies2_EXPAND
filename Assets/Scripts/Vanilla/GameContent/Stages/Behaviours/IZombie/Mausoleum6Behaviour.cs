﻿using MVZ2.Vanilla.Level;
using MVZ2Logic.IZombie;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    public class Mausoleum6Behaviour : IZombieBehaviour
    {
        public Mausoleum6Behaviour(StageDefinition stageDef) : base(stageDef)
        {
        }
        protected override NamespaceID GetNewLayout(int round, RandomGenerator rng)
        {
            switch (round)
            {
                case 0:
                    return layout1;
                case 1:
                    return layout2;
                default:
                    return layout3;
            }
        }

        protected override int GetMaxRounds()
        {
            return 3;
        }
        protected override void ReplaceBlueprints(LevelEngine level, IZombieLayoutDefinition layout)
        {
            if (layout == null)
                return;
            level.FillSeedPacks(layout.Blueprints);
        }
        private NamespaceID layout1 = VanillaIZombieLayoutID.dispenserPunchton4;
        private NamespaceID layout2 = VanillaIZombieLayoutID.highAndLow4;
        private NamespaceID layout3 = VanillaIZombieLayoutID.redAlert5;
    }
}
