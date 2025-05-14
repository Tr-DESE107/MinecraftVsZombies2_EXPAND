using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class EntityPlaceMethod : PlaceMethod
    {
        public override NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            return placement.GetSpawnError(grid, entity);
        }
        public override Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity, PlaceParams param)
        {
            if (param.IsCommandBlock())
            {
                var spawnParam = CommandBlock.GetImitateSpawnParams(entity.GetID());
                var commandBlock = grid.SpawnPlacedEntity(VanillaContraptionID.commandBlock, spawnParam);
                return commandBlock;
            }
            return grid.SpawnPlacedEntity(entity.GetID());
        }
    }
}
