#nullable enable

using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [AutoStageDefinition(VanillaStageNames.BlueprintBossRush)]
    public partial class BlueprintBossRush : StageDefinition
    {
        public BlueprintBossRush(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new BossRushBehaviour(this));
        }

        
    }
}
