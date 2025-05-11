using MVZ2.Vanilla.Grids;
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
        public override Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            return grid.SpawnPlacedEntity(entity.GetID());
        }
    }
}
