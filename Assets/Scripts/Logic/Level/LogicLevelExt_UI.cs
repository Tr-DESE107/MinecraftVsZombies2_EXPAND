using System;
using MVZ2Logic.Level.Components;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
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
    }
}
