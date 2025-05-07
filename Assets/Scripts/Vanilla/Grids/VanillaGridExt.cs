using System.Linq;
using System.Security.Cryptography;
using MVZ2.GameContent.Grids;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.HeldItems;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.SeedPacks;
using PVZEngine.Triggers;
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

        public static HeldHighlight GetSeedHeldHighlight(this LawnGrid grid, SeedDefinition seedDef)
        {
            if (seedDef != null)
            {
                var seedID = seedDef.GetID();
                if (grid.CanPlaceOrStackBlueprint(seedID, out _))
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
        /// <summary>
        /// 在一个网格的位置上放置一个蓝图。
        /// 如果是实体蓝图，则会调用<see cref="PlaceBlueprintEntity"/>，放置一个实体，或在已有实体上堆叠。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="seedDef"></param>
        /// <param name="heldItemData"></param>
        public static void UseEntityBlueprint(this LawnGrid grid, SeedPack seed, IHeldItemData heldItemData)
        {
            var seedDef = seed?.Definition;
            if (seedDef == null)
                return;
            var entity = grid.PlaceBlueprintEntity(seedDef, heldItemData);
            var level = grid.Level;
            level.Triggers.RunCallback(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, c => c(entity, seedDef, seed, heldItemData));
        }
        #endregion

        #region 放置蓝图定义
        /// <summary>
        /// 在一个网格的位置上放置一个蓝图。
        /// 如果是实体蓝图，则会调用<see cref="PlaceBlueprintEntity"/>，放置一个实体，或在已有实体上堆叠。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="seedDef"></param>
        /// <param name="heldItemData"></param>
        public static void UseEntityBlueprintDefinition(this LawnGrid grid, SeedDefinition seedDef, IHeldItemData heldItemData)
        {
            if (seedDef == null)
                return;
            var entity = grid.PlaceBlueprintEntity(seedDef, heldItemData);
            var level = grid.Level;
            level.Triggers.RunCallback(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, c => c(entity, seedDef, null, heldItemData));
        }
        #endregion

        #region 放置或堆叠实体
        public static bool CanPlaceOrStackBlueprint(this LawnGrid grid, NamespaceID seedID, out NamespaceID error)
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
        public static bool CanPlaceOrStackEntity(this LawnGrid grid, NamespaceID entityID)
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
        /// <summary>
        /// 放置一个蓝图的实体，或升级器械，或在已有实体上堆叠。
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="seedDef"></param>
        /// <param name="heldItemData"></param>
        /// <returns></returns>
        public static Entity PlaceBlueprintEntity(this LawnGrid grid, SeedDefinition seedDef, IHeldItemData heldItemData)
        {
            var level = grid.Level;
            var entityID = seedDef.GetSeedEntityID();
            var entityDef = level.Content.GetEntityDefinition(entityID);
            if (entityDef == null)
                return null;
            var stackOnEntity = entityDef.GetStackOnEntity();
            if (NamespaceID.IsValid(stackOnEntity))
            {
                var entity = grid.GetEntities().FirstOrDefault(e => e.IsEntityOf(stackOnEntity));
                if (entity != null && entity.Exists() && entity.CanStackFrom(entityID))
                {
                    entity.StackFromEntity(entityID);
                    return entity;
                }
            }
            var upgradeFromEntity = entityDef.GetUpgradeFromEntity();
            if (NamespaceID.IsValid(upgradeFromEntity))
            {
                var entity = grid.GetEntities().FirstOrDefault(e => e.IsEntityOf(upgradeFromEntity));
                if (entity != null && entity.Exists())
                {
                    return entity.UpgradeToContraption(entityID);
                }
            }
            return grid.PlaceEntity(entityID);
        }

        #endregion

        #region 放置实体
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

            grid.PostPlaceEntity(entity);

            return entity;
        }
        public static void PostPlaceEntity(this LawnGrid grid, Entity entity)
        {
            var level = grid.Level;
            level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_PLACE_ENTITY, entity.GetDefinitionID(), c => c(grid, entity));
        }
        #endregion
    }
}
