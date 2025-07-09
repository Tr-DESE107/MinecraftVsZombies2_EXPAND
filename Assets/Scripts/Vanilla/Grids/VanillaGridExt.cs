﻿using MVZ2.GameContent.Placements;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.HeldItems;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Placements;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridExt
    {
        public static HeldHighlight GetSeedHeldHighlight(this LawnGrid grid, SeedDefinition seedDef)
        {
            if (seedDef != null)
            {
                var seedID = seedDef.GetID();
                if (grid.CanPlaceBlueprint(seedID, out _))
                {
                    return HeldHighlight.Green();
                }
                else
                {
                    return HeldHighlight.Red();
                }
            }
            return HeldHighlight.None;
        }

        #region 层级
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
        public static Entity GetToolEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetLayerEntity(VanillaGridLayers.tool);
        }
        public static Entity GetProtectorEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetLayerEntity(VanillaGridLayers.protector);
        }
        #endregion

        #region 放置音效
        public static NamespaceID GetPlaceSound(this LawnGrid grid, Entity entity)
        {
            return grid.Definition.GetPlaceSound(entity);
        }
        #endregion

        #region 放置蓝图
        public static void UseEntityBlueprint(this LawnGrid grid, SeedPack seed, IHeldItemData heldItemData)
        {
            var seedDef = seed?.Definition;
            if (seedDef == null)
                return;
            var param = new PlaceParams();
            param.SetCommandBlock(seed.IsCommandBlock());
            var entity = grid.PlaceEntityBlueprint(seedDef, param);
            if (entity == null)
                return;
            var level = grid.Level;
            level.Triggers.RunCallback(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, new VanillaLevelCallbacks.PostUseEntityBlueprintParams(entity, seedDef, seed, heldItemData));
        }
        #endregion

        #region 放置蓝图定义
        /// <summary>
        /// 在一个网格的位置上放置一个蓝图。
        /// 如果是实体蓝图，则会调用<see cref="PlaceEntityBlueprint"/>，放置一个实体，或在已有实体上堆叠。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="seedDef"></param>
        /// <param name="heldItemData"></param>
        public static void UseEntityBlueprintDefinition(this LawnGrid grid, SeedDefinition seedDef, IHeldItemData heldItemData, bool isCommandBlock = false)
        {
            if (seedDef == null)
                return;
            var param = new PlaceParams();
            param.SetCommandBlock(isCommandBlock);
            var entity = grid.PlaceEntityBlueprint(seedDef, param);
            var level = grid.Level;
            level.Triggers.RunCallback(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, new VanillaLevelCallbacks.PostUseEntityBlueprintParams(entity, seedDef, null, heldItemData));
        }
        #endregion

        #region 生成实体
        public static bool CanSpawnEntity(this LawnGrid grid, NamespaceID entityID)
        {
            return grid.GetEntitySpawnStatus(entityID) == null;
        }
        public static NamespaceID GetEntitySpawnStatus(this LawnGrid grid, NamespaceID entityID)
        {
            var level = grid.Level;
            var entityDef = level.Content.GetEntityDefinition(entityID);
            return grid.GetEntitySpawnStatus(entityDef);
        }
        public static NamespaceID GetEntitySpawnStatus(this LawnGrid grid, EntityDefinition entityDef)
        {
            if (entityDef == null)
                return null;
            var level = grid.Level;
            // 可放置。
            var placementID = entityDef.GetPlacementID();
            var placementDef = level.Content.GetPlacementDefinition(placementID);
            if (placementDef == null)
                return null;
            return placementDef.GetSpawnError(grid, entityDef);
        }
        #endregion
        #region 放置实体
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
        public static bool CanPlaceEntity(this LawnGrid grid, NamespaceID entityID)
        {
            return grid.GetEntityPlaceStatus(entityID) == null;
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
            var level = grid.Level;
            // 可放置。
            var placementID = entityDef.GetPlacementID();
            var placementDef = level.Content.GetPlacementDefinition(placementID);
            if (placementDef == null)
                return null;
            return placementDef.GetPlaceError(grid, entityDef);
        }
        public static Entity PlaceEntityBlueprint(this LawnGrid grid, SeedDefinition seedDef, PlaceParams param)
        {
            return PlaceEntity(grid, seedDef.GetSeedEntityID(), param);
        }
        public static Entity PlaceEntity(this LawnGrid grid, NamespaceID entityID, PlaceParams param)
        {
            var level = grid.Level;
            var entityDef = level.Content.GetEntityDefinition(entityID);
            return grid.PlaceEntity(entityDef, param);
        }
        public static Entity PlaceEntity(this LawnGrid grid, EntityDefinition entityDef, PlaceParams param)
        {
            var level = grid.Level;
            if (entityDef == null)
                return null;
            var placementID = entityDef.GetPlacementID();
            var placement = level.Content.GetPlacementDefinition(placementID);
            return grid.PlaceEntity(entityDef, placement, param);
        }
        public static Entity PlaceEntity(this LawnGrid grid, EntityDefinition entityDef, PlacementDefinition placement, PlaceParams param)
        {
            if (entityDef == null || placement == null)
                return null;
            return placement.PlaceEntity(grid, entityDef, param);
        }
        public static Entity SpawnPlacedEntity(this LawnGrid grid, NamespaceID entityID, SpawnParams param = null)
        {
            if (grid == null)
                return null;
            if (!grid.PrePlaceEntity(entityID))
                return null;

            var level = grid.Level;
            var x = level.GetEntityColumnX(grid.Column);
            var z = level.GetEntityLaneZ(grid.Lane);
            var y = level.GetGroundY(x, z);

            var position = new Vector3(x, y, z);
            var entityDef = level.Content.GetEntityDefinition(entityID);
            var entity = level.Spawn(entityID, position, null, param);
            entity.PlaySound(grid.GetPlaceSound(entity));

            grid.PostPlaceEntity(entity);

            return entity;
        }
        private static bool PrePlaceEntity(this LawnGrid grid, NamespaceID entityID)
        {
            var level = grid.Level;
            var result = new CallbackResult(true);
            level.Triggers.RunCallbackWithResultFiltered(VanillaLevelCallbacks.PRE_PLACE_ENTITY, new VanillaLevelCallbacks.PlaceEntityParams(grid, entityID), result, entityID);
            return result.GetValue<bool>();
        }
        private static void PostPlaceEntity(this LawnGrid grid, Entity entity)
        {
            var level = grid.Level;
            level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_PLACE_ENTITY, new VanillaLevelCallbacks.PostPlaceEntityParams(grid, entity), entity.GetDefinitionID());
        }
        #endregion
    }
}
