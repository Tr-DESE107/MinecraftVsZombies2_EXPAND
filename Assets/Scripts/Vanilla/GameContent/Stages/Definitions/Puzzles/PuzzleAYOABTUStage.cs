using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleAllYourObservesAreBelongToUs)]
    public class PuzzleAYOABTUStage : IZombiePuzzleStage
    {
        public PuzzleAYOABTUStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleAllYourObservesAreBelongToUs)
        {
        }
    }
}
