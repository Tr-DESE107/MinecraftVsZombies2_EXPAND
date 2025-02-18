using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.castle11)]
    public partial class WitherStage : StageDefinition
    {
        public WitherStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new WitherStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new ConveyorStageBehaviour(this));

            this.SetClearSound(VanillaSoundID.finalItem);
        }
    }
}
