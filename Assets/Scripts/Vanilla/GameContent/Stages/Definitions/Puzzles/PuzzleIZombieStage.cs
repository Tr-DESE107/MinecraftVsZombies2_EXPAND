using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleIZombie)]
    public class PuzzleIZombieStage : IZombiePuzzleStage
    {
        public PuzzleIZombieStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleIZombie)
        {
        }
    }
}
