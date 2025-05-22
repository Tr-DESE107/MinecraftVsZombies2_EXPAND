using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleISkeleton)]
    public class PuzzleISkeletonStage : IZombiePuzzleStage
    {
        public PuzzleISkeletonStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleISkeleton)
        {
        }
    }
}
