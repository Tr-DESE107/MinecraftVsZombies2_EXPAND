using MVZ2.HeldItems;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Vanilla.HeldItems
{
    [Definition(BuiltinHeldItemNames.conveyor)]
    public class ConveyorBlueprintHeldItemDefinition : BlueprintHeldItemDefinition
    {
        public ConveyorBlueprintHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void OnUseBlueprint(LawnGrid grid, IHeldItemData data, SeedPack seed)
        {
            base.OnUseBlueprint(grid, data, seed);
            seed.Level.RemoveConveyorSeedPackAt((int)data.ID);
        }

        protected override SeedPack GetSeedPackAt(LevelEngine level, int index)
        {
            return level.GetConveyorSeedPackAt(index);
        }
    }
}
