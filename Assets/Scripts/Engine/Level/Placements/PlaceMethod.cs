using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Callbacks;

namespace PVZEngine.Placements
{
    public abstract class PlaceMethod
    {
        public abstract NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef);
        public abstract Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef);
    }
}
