using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
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
        protected override void PostPlaceEntity(LawnGrid grid, IHeldItemData data, SeedPack seed, Entity entity)
        {
            base.PostPlaceEntity(grid, data, seed, entity);
            var drawnFromPool = seed.GetDrawnConveyorSeed();
            if (NamespaceID.IsValid(drawnFromPool))
            {
                entity.TakenConveyorSeed = drawnFromPool;
            }
            seed.Level.RemoveConveyorSeedPackAt((int)data.ID);
        }

        protected override SeedPack GetSeedPackAt(LevelEngine level, int index)
        {
            return level.GetConveyorSeedPackAt(index);
        }
    }
}
