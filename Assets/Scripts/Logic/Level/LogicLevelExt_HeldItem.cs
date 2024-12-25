using MVZ2.HeldItems;
using MVZ2Logic.Callbacks;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        #region 手持物品
        public static IHeldItemComponent GetHeldItemComponent(this LevelEngine level)
        {
            return level.GetComponent<IHeldItemComponent>();
        }
        public static void SetHeldItem(this LevelEngine level, NamespaceID type, int id, int priority, bool noCancel = false)
        {
            var component = level.GetHeldItemComponent();
            component.SetHeldItem(type, id, priority, noCancel);
        }
        public static void SetHeldItem(this LevelEngine level, IHeldItemData data)
        {
            var component = level.GetHeldItemComponent();
            component.SetHeldItem(data);
        }
        public static void ResetHeldItem(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            component.ResetHeldItem();
        }
        public static bool CancelHeldItem(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.CancelHeldItem();
        }
        public static NamespaceID GetHeldItemType(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.Data.Type;
        }
        public static long GetHeldItemID(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.Data.ID;
        }
        public static IHeldItemData GetHeldItemData(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.Data;
        }
        public static HeldFlags GetHeldFlagsOnEntity(this LevelEngine level, Entity entity, NamespaceID heldType, IHeldItemData data)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.GetHeldFlagsOnEntity(entity, data);
        }
        public static HeldFlags GetHeldFlagsOnGrid(this LevelEngine level, LawnGrid grid, NamespaceID heldType, IHeldItemData data)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.GetHeldFlagsOnGrid(grid, data);
        }
        public static string GetHeldErrorMessageOnGrid(this LevelEngine level, LawnGrid grid, NamespaceID heldType, IHeldItemData data)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.GetHeldErrorMessageOnGrid(grid, data);
        }
        public static bool IsHeldItemForGrid(this LevelEngine level, NamespaceID heldType)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.IsForGrid();
        }
        public static bool IsHeldItemForEntity(this LevelEngine level, NamespaceID heldType)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.IsForEntity();
        }
        public static bool IsHeldItemForPickup(this LevelEngine level, NamespaceID heldType)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.IsForPickup();
        }
        public static bool UseOnEntity(this LevelEngine level, Entity entity, NamespaceID heldType, IHeldItemData data, PointerPhase phase)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.UseOnEntity(entity, data, phase);
        }
        public static bool UseOnGrid(this LevelEngine level, LawnGrid grid, NamespaceID heldType, IHeldItemData data, PointerPhase phase, NamespaceID targetLayer)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.UseOnGrid(grid, data, phase, targetLayer);
        }
        public static void UseOnLawn(this LevelEngine level, LawnArea area, NamespaceID heldType, IHeldItemData data, PointerPhase phase)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            heldItemDef.UseOnLawn(level, area, data, phase);
        }
        public static HeldFlags GetHeldFlagsOnEntity(this LevelEngine level, Entity entity)
        {
            return level.GetHeldFlagsOnEntity(entity, level.GetHeldItemType(), level.GetHeldItemData());
        }
        public static HeldFlags GetHeldFlagsOnGrid(this LevelEngine level, LawnGrid grid)
        {
            return level.GetHeldFlagsOnGrid(grid, level.GetHeldItemType(), level.GetHeldItemData());
        }
        public static string GetHeldErrorMessageOnGrid(this LevelEngine level, LawnGrid grid)
        {
            return level.GetHeldErrorMessageOnGrid(grid, level.GetHeldItemType(), level.GetHeldItemData());
        }
        public static bool UseOnEntity(this LevelEngine level, Entity entity, PointerPhase phase)
        {
            return level.UseOnEntity(entity, level.GetHeldItemType(), level.GetHeldItemData(), phase);
        }
        public static bool UseOnGrid(this LevelEngine level, LawnGrid grid, PointerPhase phase, NamespaceID targetLayer)
        {
            return level.UseOnGrid(grid, level.GetHeldItemType(), level.GetHeldItemData(), phase, targetLayer);
        }
        public static void UseOnLawn(this LevelEngine level, LawnArea area, PointerPhase phase)
        {
            level.UseOnLawn(area, level.GetHeldItemType(), level.GetHeldItemData(), phase);
        }
        public static IModelInterface GetHeldItemModelInterface(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.GetHeldItemModelInterface();
        }
        #endregion
    }
}
