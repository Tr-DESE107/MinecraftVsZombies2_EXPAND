﻿using MVZ2.HeldItems;
using MVZ2Logic.Games;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level.Components;
using PVZEngine;
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
        public static void SetHeldItem(this LevelEngine level, NamespaceID type, long id, int priority, bool noCancel = false)
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
        public static HeldHighlight GetHeldHighlight(this LevelEngine level, HeldItemTarget target, NamespaceID heldType, IHeldItemData data)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            return heldItemDef.GetHighlight(target, data);
        }
        public static void UseHeldItem(this LevelEngine level, HeldItemTarget target, NamespaceID heldType, IHeldItemData data, PointerInteraction interaction)
        {
            var heldItemDef = level.Content.GetHeldItemDefinition(heldType);
            heldItemDef.Use(target, data, interaction);
        }
        public static HeldHighlight GetHeldHighlight(this LevelEngine level, HeldItemTarget target)
        {
            return level.GetHeldHighlight(target, level.GetHeldItemType(), level.GetHeldItemData());
        }
        public static void UseHeldItem(this LevelEngine level, HeldItemTarget target, PointerInteraction interaction)
        {
            level.UseHeldItem(target, level.GetHeldItemType(), level.GetHeldItemData(), interaction);
        }
        public static IModelInterface GetHeldItemModelInterface(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.GetHeldItemModelInterface();
        }
        #endregion
    }
}
