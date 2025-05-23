using MVZ2.Vanilla.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.puzzleIZombieEndless)]
    public class PuzzleIZombieEndlessStage : StageDefinition
    {
        public PuzzleIZombieEndlessStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new IZombieEndlessBehaviour(this));

            this.SetIZombie(true);
        }
    }
}
