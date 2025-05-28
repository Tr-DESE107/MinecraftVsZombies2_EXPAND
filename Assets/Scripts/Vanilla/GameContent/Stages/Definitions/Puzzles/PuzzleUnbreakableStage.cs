﻿using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleUnbreakable)]
    public class PuzzleUnbreakableStage : IZombiePuzzleStage
    {
        public PuzzleUnbreakableStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleUnbreakable)
        {
        }
    }
}
