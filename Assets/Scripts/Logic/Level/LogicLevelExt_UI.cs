using System;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEditor.Presets;
using UnityEngine;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static IUIComponent GetUIComponent(this LevelEngine level)
        {
            return level.GetComponent<IUIComponent>();
        }
        public static Vector3 ScreenToLawnPositionByZ(this LevelEngine level, Vector2 screenPosition, float z)
        {
            var component = level.GetUIComponent();
            return component.ScreenToLawnPositionByZ(screenPosition, z);
        }
        public static Vector3 ScreenToLawnPositionByY(this LevelEngine level, Vector2 screenPosition, float y)
        {
            var component = level.GetUIComponent();
            return component.ScreenToLawnPositionByY(screenPosition, y);
        }
        public static Vector3 ScreenToLawnPositionByRelativeY(this LevelEngine level, Vector2 screenPosition, float relativeY)
        {
            var component = level.GetUIComponent();
            return component.ScreenToLawnPositionByRelativeY(screenPosition, relativeY);
        }
        public static void SetConveyorMode(this LevelEngine level, bool value)
        {
            var component = level.GetUIComponent();
            component.SetConveyorMode(value);
        }
        public static bool IsConveyorMode(this LevelEngine level)
        {
            var component = level.GetUIComponent();
            return component.IsConveyorMode();
        }
        public static void SetMoneyFade(this LevelEngine level, bool fade)
        {
            var component = level.GetUIComponent();
            component.SetMoneyFade(fade);
        }
        public static void ShowMoney(this LevelEngine level)
        {
            var component = level.GetUIComponent();
            component.ShowMoney();
        }
        public static void SetEnergyActive(this LevelEngine level, bool visible)
        {
            var component = level.GetUIComponent();
            component.SetEnergyActive(visible);
        }
        public static void SetBlueprintsActive(this LevelEngine level, bool visible)
        {
            var component = level.GetUIComponent();
            component.SetBlueprintsActive(visible);
        }
        public static void SetPickaxeActive(this LevelEngine level, bool visible)
        {
            var component = level.GetUIComponent();
            component.SetPickaxeActive(visible);
        }
        public static void SetStarshardActive(this LevelEngine level, bool visible)
        {
            var component = level.GetUIComponent();
            component.SetStarshardActive(visible);
        }
        public static void SetTriggerActive(this LevelEngine level, bool visible)
        {
            var component = level.GetUIComponent();
            component.SetTriggerActive(visible);
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
        public static void SetHintArrowPointToTrigger(this LevelEngine level)
        {
            var component = level.GetUIComponent();
            component.SetHintArrowPointToTrigger();
        }
        public static void SetHintArrowPointToStarshard(this LevelEngine level)
        {
            var component = level.GetUIComponent();
            component.SetHintArrowPointToStarshard();
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
        public static void SetProgressBarToBoss(this LevelEngine level, NamespaceID barStyle)
        {
            var component = level.GetUIComponent();
            component.SetProgressBarToBoss(barStyle);
        }
        public static void SetProgressBarToStage(this LevelEngine level)
        {
            var component = level.GetUIComponent();
            component.SetProgressBarToStage();
        }
        public static void SetAreaModelPreset(this LevelEngine level, string preset)
        {
            var component = level.GetUIComponent();
            component.SetAreaModelPreset(preset);
        }
        public static void PauseGame(this LevelEngine level, int pauseLevel = 0)
        {
            var component = level.GetUIComponent();
            component.PauseGame(pauseLevel);
        }
        public static void ResumeGame(this LevelEngine level, int pauseLevel = 0)
        {
            var component = level.GetUIComponent();
            component.ResumeGame(pauseLevel);
        }
        public static void ShowDialog(this LevelEngine level, string title, string desc, string[] options, Action<int> onSelect = null)
        {
            var component = level.GetUIComponent();
            component.ShowDialog(title, desc, options, onSelect);
        }
    }
}
