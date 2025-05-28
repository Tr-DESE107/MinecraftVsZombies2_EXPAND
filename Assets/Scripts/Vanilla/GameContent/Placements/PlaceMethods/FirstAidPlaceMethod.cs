﻿using System.Linq;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class FirstAidPlaceMethod : PlaceMethod
    {
        public override NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            if (!Global.Game.IsUnlocked(VanillaUnlockID.obsidianFirstAid))
                return VanillaGridStatus.notUnlocked;
            if (!entity.IsDefensive())
                return VanillaGridStatus.firstAid;
            var entities = grid.GetEntities();
            if (entities.Count() <= 0 || !entities.Any(e => CanFirstAid(entity, e)))
            {
                return VanillaGridStatus.firstAid;
            }
            return null;
        }
        public override Entity PlaceEntity(PlacementDefinition placement, LawnGrid grid, EntityDefinition entityDef, PlaceParams param)
        {
            if (!entityDef.IsDefensive())
                return null;
            var entities = grid.GetEntities();
            var entity = entities.FirstOrDefault(e => CanFirstAid(entityDef, e));
            if (entity.ExistsAndAlive())
            {
                return entity.FirstAid();
            }
            return null;
        }
        private bool CanFirstAid(EntityDefinition entityDef, Entity entity)
        {
            if (entity.Definition != entityDef)
                return false;
            if (entity.Health >= entity.GetMaxHealth())
                return false;
            if (!entity.IsFriendlyEntity())
                return false;
            return true;
        }
    }
}
