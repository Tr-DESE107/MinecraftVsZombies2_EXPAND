﻿using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleDeadBalloon)]
    public class PuzzleDeadBalloonStage : IZombiePuzzleStage
    {
        public PuzzleDeadBalloonStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleDeadBalloon)
        {
        }
    }
}
