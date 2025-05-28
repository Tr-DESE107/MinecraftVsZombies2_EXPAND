﻿using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Games;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    public class OptionSeed : SeedDefinition
    {
        public OptionSeed(string nsp, string name, int cost) : base(nsp, name)
        {
            SetProperty(VanillaSeedProps.SEED_TYPE, SeedTypes.OPTION);
            SetProperty(VanillaSeedProps.SEED_OPTION_ID, new NamespaceID(nsp, name));
            SetProperty(EngineSeedProps.COST, (float)cost);
        }
        public override void Update(SeedPack seedPack, float rechargeSpeed)
        {
            base.Update(seedPack, rechargeSpeed);
            var optionID = seedPack.Definition.GetSeedOptionID();
            var optionDef = seedPack.Level.Content.GetSeedOptionDefinition(optionID);
            if (optionDef == null)
                return;
            optionDef.Update(seedPack, rechargeSpeed);
        }
    }
}
