﻿using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class DrivenserPlaceMethod : PlaceMethod
    {
        public override NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            var entities = grid.GetEntities();
            if (entities.Count() <= 0 || !entities.Any(e => Drivenser.CanUpgrade(e)))
            {
                return VanillaGridStatus.onlyUpgrade;
            }
            return null;
        }

        public override Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity, PlaceParams param)
        {
            var entities = grid.GetEntities();
            var drivenser = entities.FirstOrDefault(e => Drivenser.CanUpgrade(e));
            if (drivenser == null)
                return null;
            Drivenser.Upgrade(drivenser);
            return drivenser;
        }
    }
}
