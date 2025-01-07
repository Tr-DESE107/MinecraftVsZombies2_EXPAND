using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Seeds
{
    public class OptionSeed : SeedDefinition
    {
        public OptionSeed(string nsp, string name, int cost) : base(nsp, name)
        {
            SetProperty(VanillaSeedProps.SEED_TYPE, SeedTypes.OPTION);
            SetProperty(VanillaSeedProps.SEED_OPTION_ID, new NamespaceID(nsp, name));
            SetProperty(EngineSeedProps.COST, cost);
        }
    }
}
