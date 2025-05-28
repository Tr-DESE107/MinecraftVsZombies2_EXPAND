﻿using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.SeedPacks
{
    [BuffDefinition(VanillaBuffNames.SeedPack.upgradeEndlessCost)]
    public class UpgradeEndlessCostBuff : BuffDefinition
    {
        public UpgradeEndlessCostBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineSeedProps.COST, NumberOperator.Add, PROP_ADDITION));
        }
        public static readonly VanillaBuffPropertyMeta<float> PROP_ADDITION = new VanillaBuffPropertyMeta<float>("Reduction");
    }
}
