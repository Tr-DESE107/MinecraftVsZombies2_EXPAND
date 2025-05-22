using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleFireInTheHole)]
    public class PuzzleTerrorWarsStage : IZombiePuzzleStage
    {
        public PuzzleTerrorWarsStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleFireInTheHole)
        {
        }
    }
}
