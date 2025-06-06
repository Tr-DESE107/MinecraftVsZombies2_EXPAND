﻿using PVZEngine.Base;

namespace PVZEngine.Definitions
{
    public class RechargeDefinition : Definition
    {
        public RechargeDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.RECHARGE;
    }
}
