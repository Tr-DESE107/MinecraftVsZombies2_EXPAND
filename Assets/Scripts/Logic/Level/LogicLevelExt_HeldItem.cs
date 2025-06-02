﻿using MVZ2.HeldItems;
using MVZ2Logic.Games;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Entities;
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
            return level.GetHeldItemData().Type;
        }
        public static long GetHeldItemID(this LevelEngine level)
        {
            return level.GetHeldItemData().ID;
        }
        public static IHeldItemData GetHeldItemData(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.Data;
        }
        public static HeldItemDefinition GetHeldItemDefinition(this LevelEngine level)
        {
            var heldType = level.GetHeldItemType();
            return level.Content.GetHeldItemDefinition(heldType);
        }
        public static HeldHighlight GetHeldHighlight(this LevelEngine level, IHeldItemTarget target, PointerData pointer)
        {
            var data = level.GetHeldItemData();
            var heldItemDef = level.GetHeldItemDefinition();
            return heldItemDef.GetHighlight(target, data, pointer);
        }
        public static void DoHeldItemPointerEvent(this LevelEngine level, IHeldItemTarget target, PointerInteractionData pointerParams)
        {
            var data = level.GetHeldItemData();
            var heldItemDef = level.GetHeldItemDefinition();
            heldItemDef.DoPointerEvent(target, data, pointerParams);
        }
        public static IModelInterface GetHeldItemModelInterface(this LevelEngine level)
        {
            var component = level.GetHeldItemComponent();
            return component.GetHeldItemModelInterface();
        }
        public static bool ShouldHeldItemMakeEntityTwinkle(this LevelEngine level, Entity entity)
        {
            var heldDef = level.GetHeldItemDefinition();
            var heldItemData = level.GetHeldItemData();
            var behaviours = heldDef.GetBehaviours();
            foreach (var behaviour in behaviours)
            {
                if (behaviour is IHeldTwinkleEntityBehaviour twinkle && twinkle.ShouldMakeEntityTwinkle(entity, heldItemData))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
