﻿using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.soulfireBlast)]
    public class SoulfireBlast : EffectBehaviour
    {

        #region 公有方法
        public SoulfireBlast(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}