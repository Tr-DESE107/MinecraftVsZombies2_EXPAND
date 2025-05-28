﻿using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.soulfireBurn)]
    public class SoulfireBurn : EffectBehaviour
    {

        #region 公有方法
        public SoulfireBurn(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}