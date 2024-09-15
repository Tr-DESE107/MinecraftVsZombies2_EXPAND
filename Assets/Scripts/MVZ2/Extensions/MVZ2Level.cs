using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2
{
    public static class MVZ2Level
    {
        #region 手持物品
        public static HeldItemComponent GetHeldItemComponent(this LevelEngine level)
        {
            return level.GetComponent<HeldItemComponent>();
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
        public static int GetHeldItemID(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.HeldItemID;
        }
        public static bool IsEntityValidForHeldItem(this LevelEngine level, Entity entity)
        {
            return level.IsEntityValidForHeldItem(entity, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static bool IsEntityValidForHeldItem(this LevelEngine level, Entity entity, NamespaceID heldType, int heldId)
        {
            var component = level.GetHeldItemComponent();
            return component.IsValidOnEntity(entity, heldType, heldId);
        }
        public static bool IsGridValidForHeldItem(this LevelEngine level, LawnGrid grid)
        {
            return level.IsGridValidForHeldItem(grid, level.GetHeldItemType(), level.GetHeldItemID());
        }
        public static bool IsGridValidForHeldItem(this LevelEngine level, LawnGrid grid, NamespaceID heldType, int heldId)
        {
            var component = level.GetHeldItemComponent();
            return component.IsValidOnGrid(grid, heldType, heldId);
        }
        public static bool UseOnEntity(this LevelEngine level, Entity entity)
        {
            var component = level.GetHeldItemComponent();
            return component.UseOnEntity(entity);
        }
        public static void HoverOnEntity(this LevelEngine level, Entity entity)
        {
            var component = level.GetHeldItemComponent();
            component.HoverOnEntity(entity);
        }
        public static bool UseOnGrid(this LevelEngine level, LawnGrid grid)
        {
            var component = level.GetHeldItemComponent();
            return component.UseOnGrid(grid);
        }
        public static void UseOnLawn(this LevelEngine level, LawnArea area)
        {
            var component = level.GetHeldItemComponent();
            component.UseOnLawn(level, area);
        }
        public static bool IsHoldingItem(this LevelEngine level)
        {
            var type = level.GetHeldItemType();
            return NamespaceID.IsValid(type) && type != HeldTypes.none;
        }
        public static bool IsHoldingPickaxe(this LevelEngine level)
        {
            return level.GetHeldItemType() == HeldTypes.pickaxe;
        }
        public static bool IsHoldingStarshard(this LevelEngine level)
        {
            return level.GetHeldItemType() == HeldTypes.starshard;
        }
        public static bool IsHoldingBlueprint(this LevelEngine level, int i)
        {
            return level.GetHeldItemType() == HeldTypes.blueprint && level.GetHeldItemID() == i;
        }
        #endregion
    }
}
