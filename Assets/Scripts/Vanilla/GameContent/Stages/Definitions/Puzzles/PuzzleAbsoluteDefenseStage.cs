﻿using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleAbsoluteDefense)]
    public class PuzzleAbsoluteDefenseStage : IZombiePuzzleStage
    {
        public PuzzleAbsoluteDefenseStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleAbsoluteDefense)
        {
        }
    }
}
