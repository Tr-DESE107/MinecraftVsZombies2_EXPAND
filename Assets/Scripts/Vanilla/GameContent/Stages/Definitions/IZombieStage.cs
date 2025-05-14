using MVZ2.Vanilla.Level;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Stages
{
    public partial class IZombieStage : StageDefinition
    {
        public IZombieStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new IZombieBehaviour(this));

            this.SetIZombie(true);
        }
    }
}
