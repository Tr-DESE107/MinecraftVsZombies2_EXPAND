using System.Linq;
using MVZ2.GameContent.Grids;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Triggers;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
            if (grid.Definition is not IPlaceSoundGrid soundGrid)
                return entity.GetPlaceSound();
            return soundGrid.GetPlaceSound(entity);
        }
        public static bool CanPlaceEntity(this LawnGrid grid, NamespaceID entityID)
        {
            var status = grid.GetEntityPlaceStatus(entityID);
            return status == null;
        }
        public static NamespaceID GetEntityPlaceStatus(this LawnGrid grid, NamespaceID entityID)
        {
            var error = new TriggerResultNamespaceID();
            var level = grid.Level;
            var entityDef = level.Content.GetEntityDefinition(entityID);
            if (entityDef == null)
                return null;
            if (grid.GetEntities().Any(e => e.CanStackFrom(entityID)))
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
            }
            foreach (var trigger in level.Triggers.GetTriggers(VanillaLevelCallbacks.CAN_PLACE_ENTITY))
            {
                if (!trigger.Filter(entityID))
                    continue;
                trigger.Run(grid, entityID, error);
            }
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
                error = GetEntityPlaceStatus(grid, entityID);
                return error == null;
            }
            return false;
        }
        public static void PrePlaceEntity(this LawnGrid grid, NamespaceID entityID, TriggerResultBoolean cancelPlace)
        {
            if (grid == null)
                return;
            var level = grid.Level;
            foreach (var trigger in level.Triggers.GetTriggers(VanillaLevelCallbacks.PRE_PLACE_ENTITY))
            {
                if (!trigger.Filter(entityID))
                    continue;
                trigger.Run(grid, entityID, cancelPlace);
            }
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
            level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_PLACE_ENTITY, entity.GetDefinitionID(), grid, entity);
            return entity;
        }
    }
}
