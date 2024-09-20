using System;
using MVZ2.Level.Components;
using MVZ2.Vanilla;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2
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
        public static Vector2 GetEnergySlotEntityPosition(this LevelEngine level)
        {
            if (MainManager.Instance.IsMobile())
            {
                var x = 160 + BuiltinLevel.ENERGY_SLOT_WIDTH * 0.5f;
                var y = BuiltinLevel.GetScreenHeight() - BuiltinLevel.ENERGY_SLOT_WIDTH * 0.5f;
                return new Vector2(x, y);
            }
            else
            {
                var x = BuiltinLevel.GetBorderX(false) + BuiltinLevel.ENERGY_SLOT_WIDTH * 0.5f;
                var y = BuiltinLevel.GetScreenHeight() - BuiltinLevel.ENERGY_SLOT_WIDTH * 0.5f;
                return new Vector2(x, y);
            }
        }
    }
}
