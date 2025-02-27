using System.Linq;
using MVZ2.GameContent.Grids;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Triggers;
using UnityEngine;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridExt
    {
        public static bool IsWater(this LawnGrid grid)
        {
            return grid.Definition.GetID() == VanillaGridID.water;
        }
        public static Entity GetCarrierEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetLayerEntity(VanillaGridLayers.carrier);
        }
        public static Entity GetMainEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetLayerEntity(VanillaGridLayers.main);
        }
        public static Entity GetProtectorEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetLayerEntity(VanillaGridLayers.protector);
        }
        public static NamespaceID GetPlaceSound(this LawnGrid grid, Entity entity)
        {
            return grid.Definition.GetPlaceSound(entity);
        }
        public static bool CanPlaceEntity(this LawnGrid grid, NamespaceID entityID)
        {
            var status = grid.GetEntityPlaceOrStackStatus(entityID);
            return status == null;
        }
        public static NamespaceID GetEntityPlaceOrStackStatus(this LawnGrid grid, NamespaceID entityID)
        {
            var error = new TriggerResultNamespaceID();
            var level = grid.Level;
            var entityDef = level.Content.GetEntityDefinition(entityID);
            if (entityDef == null)
                return null;
            if (grid.GetEntities().Any(e => e.CanStackFrom(entityID) && e.IsFriendlyEntity()))
            {
                // 可堆叠
                error.Result = null;
            }
            else
            {
                // 可放置。
                var placementID = entityDef.GetPlacementID();
                var placementDef = level.Content.GetPlacementDefinition(placementID);
                if (placementDef == null)
                    return null;
                placementDef.CanPlaceEntityOnGrid(grid, entityDef, error);
                level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.CAN_PLACE_ENTITY, entityID, error, c => c(grid, entityID, error));
            }
            return error.Result;
        }
        public static NamespaceID GetEntityPlaceStatus(this LawnGrid grid, NamespaceID entityID)
        {
            var level = grid.Level;
            var entityDef = level.Content.GetEntityDefinition(entityID);
            return grid.GetEntityPlaceStatus(entityDef);
        }
        public static NamespaceID GetEntityPlaceStatus(this LawnGrid grid, EntityDefinition entityDef)
        {
            if (entityDef == null)
                return null;
            var error = new TriggerResultNamespaceID();
            var level = grid.Level;
            // 可放置。
            var placementID = entityDef.GetPlacementID();
            var placementDef = level.Content.GetPlacementDefinition(placementID);
            if (placementDef == null)
                return null;
            placementDef.CanPlaceEntityOnGrid(grid, entityDef, error);
            var entityID = entityDef.GetID();
            level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.CAN_PLACE_ENTITY, entityID, error, c => c(grid, entityID, error));
            return error.Result;
        }
        public static bool CanPlaceBlueprint(this LawnGrid grid, NamespaceID seedID, out NamespaceID error)
        {
            error = null;
            if (!NamespaceID.IsValid(seedID))
                return false;
            var level = grid.Level;
            var seedDef = level.Content.GetSeedDefinition(seedID);
            if (seedDef == null)
                return false;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                error = GetEntityPlaceOrStackStatus(grid, entityID);
                return error == null;
            }
            return false;
        }
        public static void PrePlaceEntity(this LawnGrid grid, NamespaceID entityID, TriggerResultBoolean cancelPlace)
        {
            if (grid == null)
                return;
            var level = grid.Level;
            level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.PRE_PLACE_ENTITY, entityID, cancelPlace, c => c(grid, entityID, cancelPlace));
        }
        public static Entity PlaceEntity(this LawnGrid grid, NamespaceID entityID)
        {
            var result = new TriggerResultBoolean();
            grid.PrePlaceEntity(entityID, result);
            if (result.Result)
                return null;

            var level = grid.Level;
            var x = level.GetEntityColumnX(grid.Column);
            var z = level.GetEntityLaneZ(grid.Lane);
            var y = level.GetGroundY(x, z);

            var position = new Vector3(x, y, z);
            var entityDef = level.Content.GetEntityDefinition(entityID);
            var entity = level.Spawn(entityID, position, null);
            entity.PlaySound(grid.GetPlaceSound(entity));
            level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_PLACE_ENTITY, entity.GetDefinitionID(), c => c(grid, entity));
            return entity;
        }
    }
}
