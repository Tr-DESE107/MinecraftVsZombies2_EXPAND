﻿using PVZEngine.Definitions;

namespace MVZ2.GameContent.Stages
{
    public partial class ClassicStage : StageDefinition
    {
        public ClassicStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
        }
    }
}
