using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace PVZEngine.Placements
{
    public abstract class PlaceMethod
    {
        public abstract NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef);
        public abstract Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef, PlaceParams param);
    }
}
