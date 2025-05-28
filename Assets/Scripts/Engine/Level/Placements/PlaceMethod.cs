﻿using PVZEngine.Entities;
using PVZEngine.Grids;

namespace PVZEngine.Placements
{
    public abstract class PlaceMethod
    {
        public abstract NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef);
        public abstract Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef, PlaceParams param);
    }
}
