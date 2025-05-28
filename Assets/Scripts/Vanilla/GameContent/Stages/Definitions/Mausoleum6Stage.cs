﻿using MVZ2.Vanilla.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.mausoleum6)]
    public class Mausoleum6Stage : StageDefinition
    {
        public Mausoleum6Stage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new Mausoleum6Behaviour(this));

            this.SetIZombie(true);
        }
    }
}
