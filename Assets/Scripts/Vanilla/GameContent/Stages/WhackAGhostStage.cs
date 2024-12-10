using PVZEngine.Definitions;

namespace MVZ2.GameContent.Stages
{
    public partial class WhackAGhostStage : StageDefinition
    {
        public WhackAGhostStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
        }
    }
}
