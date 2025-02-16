using MVZ2.Vanilla.Level;
using PVZEngine.Definitions;

namespace MVZ2.GameContent.Stages
{
    public partial class EndlessStage : StageDefinition
    {
        public EndlessStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this) { HasFinalWave = false });
            AddBehaviour(new EndlessStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));

            SetProperty(VanillaStageProps.ENDLESS, true);
        }
    }
}
