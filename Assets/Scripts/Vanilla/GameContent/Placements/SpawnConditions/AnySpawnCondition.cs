﻿using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class AnySpawnCondition : SpawnCondition
    {
        public override NamespaceID GetSpawnError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            return null;
        }
    }
}
