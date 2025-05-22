using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleMineclear)]
    public class PuzzleMineclearStage : IZombiePuzzleStage
    {
        public PuzzleMineclearStage(string nsp, string name) : base(nsp, name, VanillaIZombieLayoutID.puzzleMineclear)
        {
        }
    }
}
