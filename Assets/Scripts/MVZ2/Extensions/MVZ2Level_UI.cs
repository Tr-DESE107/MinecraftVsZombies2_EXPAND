using System;
using MVZ2.GameContent;
using MVZ2.Level.Components;
using MVZ2.Managers;
using MVZ2.Vanilla;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
    {
        public static IUIComponent GetUIComponent(this LevelEngine level)
        {
            return level.GetComponent<IUIComponent>();
        }
        public static void ShowMoney(this LevelEngine level)
        {
            var component = level.GetUIComponent();
            component.ShowMoney();
        }
        public static void ShowDialog(this LevelEngine level, string title, string desc, string[] options, Action<int> onSelect)
        {
            var component = level.GetUIComponent();
            component.ShowDialog(title, desc, options, onSelect);
        }
        public static void ShakeScreen(this LevelEngine level, float startAmplitude, float endAmplitude, int time)
        {
            var component = level.GetUIComponent();
            component.ShakeScreen(startAmplitude, endAmplitude, time);
        }
        public static void SetHintArrowPointToBlueprint(this LevelEngine level, int index)
        {
            var component = level.GetUIComponent();
            component.SetHintArrowPointToBlueprint(index);
        }
        public static void SetHintArrowPointToPickaxe(this LevelEngine level)
        {
            var component = level.GetUIComponent();
            component.SetHintArrowPointToPickaxe();
        }
        public static void SetHintArrowPointToEntity(this LevelEngine level, Entity entity)
        {
            var component = level.GetUIComponent();
            component.SetHintArrowPointToEntity(entity);
        }
        public static void HideHintArrow(this LevelEngine level)
        {
            var component = level.GetUIComponent();
            component.HideHintArrow();
        }
        public static float GetLeftUIBorderX(this LevelEngine level)
        {
            if (Global.IsMobile())
            {
                return 160;
            }
            return BuiltinLevel.GetBorderX(false);
        }
        public static Vector2 GetMoneyPanelEntityPosition(this LevelEngine level)
        {
            var x = level.GetLeftUIBorderX() + BuiltinLevel.MONEY_PANEL_X_TO_LEFT;
            var y = BuiltinLevel.MONEY_PANEL_Y_TO_BOTTOM;
            return new Vector2(x, y);
        }
        public static Vector2 GetStarshardEntityPosition(this LevelEngine level)
        {
            var x = level.GetLeftUIBorderX() + BuiltinLevel.STARSHARD_X_TO_LEFT;
            var y = BuiltinLevel.STARSHARD_Y_TO_BOTTOM;
            return new Vector2(x, y);
        }
        public static Vector2 GetEnergySlotEntityPosition(this LevelEngine level)
        {
            var x = level.GetLeftUIBorderX() + BuiltinLevel.ENERGY_SLOT_WIDTH * 0.5f;
            var y = BuiltinLevel.GetScreenHeight() - BuiltinLevel.ENERGY_SLOT_WIDTH * 0.5f;
            return new Vector2(x, y);
        }
        public static Vector2 GetScreenCenterPosition(this LevelEngine level)
        {
            var x = level.GetLeftUIBorderX() + BuiltinLevel.SCREEN_WIDTH * 0.5f;
            var y = BuiltinLevel.SCREEN_HEIGHT * 0.5f;
            return new Vector2(x, y);
        }
    }
}
