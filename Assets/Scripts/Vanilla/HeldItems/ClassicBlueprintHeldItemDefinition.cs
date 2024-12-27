using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.HeldItems
{
    [Definition(BuiltinHeldItemNames.blueprint)]
    public class ClassicBlueprintHeldItemDefinition : BlueprintHeldItemDefinition
    {
        public ClassicBlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        protected override SeedPack GetSeedPackAt(LevelEngine level, int index)
        {
            return level.GetSeedPackAt(index);
        }
        protected override void OnUseBlueprint(LawnGrid grid, IHeldItemData data, SeedPack seed)
        {
            base.OnUseBlueprint(grid, data, seed);
            var level = grid.Level;
            var seedDef = seed.Definition;
            level.AddEnergy(-seedDef.GetCost());
            level.SetRechargeTimeToUsed(seed);
            seed.ResetRecharge();
        }
    }
}
