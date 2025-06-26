﻿using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.iZombieDebug)]
    public class IZombieDebugStage : IZombiePuzzleStage
    {
        public IZombieDebugStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.iZombieDebug)
        {
        }
    }
}
