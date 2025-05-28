using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Stages
{
    public abstract class IZombiePuzzleStage : StageDefinition
    {
        public IZombiePuzzleStage(string nsp, string name, NamespaceID layout) : base(nsp, name)
        {
            AddBehaviour(new IZombiePuzzleBehaviour(this, layout));

            this.SetIZombie(true);
        }
    }
}
