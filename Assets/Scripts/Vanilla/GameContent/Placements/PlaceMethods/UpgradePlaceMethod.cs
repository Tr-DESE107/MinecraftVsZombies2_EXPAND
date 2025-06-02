﻿using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class UpgradePlaceMethod : PlaceMethod
    {
        public override NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            var entities = grid.GetEntities();
            if (entities.Count() <= 0 || !entities.Any(e => e.CanUpgradeToContraption(entity) && e.IsFriendlyEntity()))
            {
                return VanillaGridStatus.onlyUpgrade;
            }
            return null;
        }
        public override Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef, PlaceParams param)
        {
            var entity = grid.GetEntities().FirstOrDefault(e => e.CanUpgradeToContraption(entityDef));
            if (entity != null && entity.Exists())
            {
                var ent = entity.UpgradeToContraption(entityDef.GetID());
                if (ent != null && param.IsCommandBlock())
                {
                    ent.AddBuff<ImitatedBuff>();
                }
                return ent;
            }
            return null;
        }
    }
}
