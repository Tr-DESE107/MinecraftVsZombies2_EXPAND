using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.Do_Magic_Tricks)]

    public partial class Do_Magic_Tricks : StageDefinition
    {
        public Do_Magic_Tricks(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
            AddBehaviour(new ConveyorStageBehaviour(this));

        }

    }
}
