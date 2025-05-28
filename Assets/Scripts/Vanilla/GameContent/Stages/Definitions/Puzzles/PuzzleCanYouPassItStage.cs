using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleCanYouPassIt)]
    public class PuzzleCanYouPassItStage : IZombiePuzzleStage
    {
        public PuzzleCanYouPassItStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleCanYouPassIt)
        {
        }
    }
}
