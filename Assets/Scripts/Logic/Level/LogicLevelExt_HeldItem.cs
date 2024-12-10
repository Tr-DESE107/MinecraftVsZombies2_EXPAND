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
            return component.HeldItemType;
        }
        public static long GetHeldItemID(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.HeldItemID;
        }
        public static HeldFlags GetHeldFlagsOnEntity(this LevelEngine level, Entity entity, NamespaceID heldType, long heldId)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.GetHeldFlagsOnEntity(entity, heldId);
        }
        public static HeldFlags GetHeldFlagsOnGrid(this LevelEngine level, LawnGrid grid, NamespaceID heldType, long heldId)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.GetHeldFlagsOnGrid(grid, heldId);
        }
        public static string GetHeldErrorMessageOnGrid(this LevelEngine level, LawnGrid grid, NamespaceID heldType, long heldId)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.GetHeldErrorMessageOnGrid(grid, heldId);
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
        public static bool UseOnEntity(this LevelEngine level, Entity entity, NamespaceID heldType, long heldId)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.UseOnEntity(entity, heldId);
        }
        public static void HoverOnEntity(this LevelEngine level, Entity entity, NamespaceID heldType, long heldId)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            heldItemDef.HoverOnEntity(entity, heldId);
        }
        public static bool UseOnGrid(this LevelEngine level, LawnGrid grid, NamespaceID heldType, long heldId)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.UseOnGrid(grid, heldId);
        }
        public static void UseOnLawn(this LevelEngine level, LawnArea area, NamespaceID heldType, long heldId)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            heldItemDef.UseOnLawn(level, area, heldId);
        }
        public static HeldFlags GetHeldFlagsOnEntity(this LevelEngine level, Entity entity)
        {
            return level.GetHeldFlagsOnEntity(entity, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static HeldFlags GetHeldFlagsOnGrid(this LevelEngine level, LawnGrid grid)
        {
            return level.GetHeldFlagsOnGrid(grid, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static string GetHeldErrorMessageOnGrid(this LevelEngine level, LawnGrid grid)
        {
            return level.GetHeldErrorMessageOnGrid(grid, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static bool UseOnEntity(this LevelEngine level, Entity entity)
        {
            return level.UseOnEntity(entity, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static void HoverOnEntity(this LevelEngine level, Entity entity)
        {
            level.HoverOnEntity(entity, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static bool UseOnGrid(this LevelEngine level, LawnGrid grid)
        {
            return level.UseOnGrid(grid, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static void UseOnLawn(this LevelEngine level, LawnArea area)
        {
            level.UseOnLawn(area, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static IModelInterface GetHeldItemModelInterface(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.GetHeldItemModelInterface();
        }
        #endregion
    }
}
