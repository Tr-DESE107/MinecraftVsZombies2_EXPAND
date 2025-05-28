﻿using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class DevourerSpawnCondition : ContraptionSpawnCondition
    {
        public override NamespaceID GetSpawnError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            var entities = grid.GetEntities();
            if (entities.Count() <= 0 || !entities.Any(e => Devourer.CanMill(e)))
                return VanillaGridStatus.onlyCanMill;

            // 被占用。
            return base.GetSpawnError(placement, grid, entity);
        }
    }
}
